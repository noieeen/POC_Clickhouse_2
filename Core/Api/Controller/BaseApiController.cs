using System.Net;
using Core.Api.Model;
using Core.Api.Response;
using Core.Constant;
using Core.Exceptions;
using Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Core.Api.Controller
{
    /// <summary>
    /// Base Controller class for project ที่ต้องการใช้รูปแบบการเขียน controller แบบ BCRM_Controller
    /// แต่ไม่ต้องการให้มี business logic ของ BCRM_Controller อยู่
    /// </summary>
    public abstract class BaseApiController : Microsoft.AspNetCore.Mvc.Controller
    {
        // Api - Context (Header / Token)
        // public string Api_RequestId { get; set; } = null;
        public string Api_RequestId { get; set; } = Guid.NewGuid().ToString();


        // Api - Response
        public string Status { get; set; } = CoreConstant.Api.Result_Status.Fail;
        public object Data { get; set; } = null;

        public Exception ApiException { get; set; } = null;

        // Util
        public readonly ControllerUtil _ctrl_Util;

        // BCRM - Services
        public readonly ILogger _logger;
        public readonly ICommon_Exception_Factory _common_Ex_Factory;
        public readonly IHttpContextAccessor _httpContext_Accessor;

        public BaseApiController(ILogger logger,
            ICommon_Exception_Factory common_Ex_Factory,
            IHttpContextAccessor httpContext_Accessor)
        {
            // Services
            _httpContext_Accessor =
                httpContext_Accessor ?? throw new ArgumentNullException(nameof(httpContext_Accessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _common_Ex_Factory = common_Ex_Factory ?? throw new ArgumentNullException(nameof(common_Ex_Factory));

            // Util
            _ctrl_Util = new ControllerUtil(this, _httpContext_Accessor);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Header - X-Request-Id
            StringValues api_RequestId = StringValues.Empty;
            if (context.HttpContext.Request.Headers.TryGetValue(CoreConstant.Api.Header.X_Request_Id,
                    out api_RequestId))
            {
                Api_RequestId = api_RequestId;
            }
            else
            {
                Api_RequestId = Guid.NewGuid().ToString();
            }
        }

        protected virtual JsonResult Build_JsonResp()
        {
            if (string.IsNullOrEmpty(Status) || !CoreConstant.Api.Result_Status.GetAllValues().Contains(Status))
            {
                throw new InvalidOperationException(
                    $"Invalid response status. Expected: {CoreConstant.Api.Result_Status.ToStringAllValues()}");
            }
            // if (!CDP_Core_Const.Api.Result_Status.GetAllValues().Contains(Status))
            //     throw new InvalidOperationException(
            //         $"Invalid response status, expect ({CDP_Core_Const.Api.Result_Status.ToStringAllValues()})");

            JsonResult result;
            Api_Response response;

            // Exception take priority
            if (ApiException != null)
            {
                CoreException ccEx = Build_BCRMException(ApiException);

                if (ApiException is CoreException or DataNotFoundException or BL_Exception)
                {
                    Status = CoreConstant.Api.Result_Status.Fail;
                }
                else
                {
                    Status = CoreConstant.Api.Result_Status.Error;
                }

                // response = new Api_Response()
                // {
                //     Request_Id = Api_RequestId,
                //     Status = Status,
                //     Data = Data,
                //     Error = ccEx.ApiError
                // };

                response = new Api_Response();

                response.Request_Id = Api_RequestId;
                response.Status = Status;
                response.Data = Data;
                response.Error = ccEx.ApiError;


                result = new JsonResult(response);
                result.StatusCode = (int)ccEx?.Http_StatusCode != 0 ? ccEx.Http_StatusCode : 500;

                return result;
            }
            // Status: success
            else if (Status.Equals(CoreConstant.Api.Result_Status.Success, StringComparison.OrdinalIgnoreCase))
            {
                response = new Api_Response()
                {
                    Request_Id = Api_RequestId,
                    Status = Status,
                    Data = Data
                };

                result = new JsonResult(response);
                result.StatusCode = (int)HttpStatusCode.OK;

                return result;
            }

            // Default
            response = new Api_Response()
            {
                Request_Id = Api_RequestId,
                Status = Status,
                Data = Data
            };

            result = new JsonResult(response);
            result.StatusCode = (int)HttpStatusCode.BadRequest;

            return result;
        }

        protected virtual JsonResult Build_BadRequest()
        {
            CoreException apiEx = _common_Ex_Factory.Build(1000400);

            // Build
            if (!ModelState.IsValid)
            {
                apiEx.ApiError.Validate = ModelState_ResultBuilder.Init(ModelState).Build();
            }

            Api_Response apiResp = new Api_Response()
            {
                Request_Id = Api_RequestId,
                Status = CoreConstant.Api.Result_Status.Fail,
                Error = apiEx.ApiError
            };

            return new JsonResult(apiResp) { StatusCode = apiEx.Http_StatusCode };
        }

        protected virtual CoreException Build_BCRMException(Exception exception)
        {
            if (exception == null) return null;

            // Case API Controller specified, Exception
            CoreException coreException;
            if (exception is CoreException)
            {
                coreException = (CoreException)exception;

                // normally CDP_Exception already logged by BCRM_Api_Logging
                LogUtil.LogException(_logger, LogLevel.Debug, tag: "", ex: exception);
            }
            else
            {
                if (exception is DataNotFoundException)
                {
                    coreException = ConvertToCdpException((DataNotFoundException)exception);
                }
                else if (exception is BL_Exception)
                {
                    coreException = ConvertToCdpException((BL_Exception)exception);
                }
                else if (exception is BadRequestException)
                {
                    coreException = ConvertToCdpException((BadRequestException)exception);
                }
                else if (exception is AppException)
                {
                    coreException = ConvertToCdpException((AppException)exception);
                }
                else
                {
                    // Internal Server Error
                    // ErrorId: 1000500 | Namespace: BCRM-Core [10] Module: Common [00] Action: Internal Server Error [500]
                    coreException = _common_Ex_Factory.Build(1000500);
                }

                LogUtil.LogException(_logger, tag: "", ex: exception);
            }

            return coreException;
        }

        protected virtual IActionResult Redirect(string baseUrl, Dictionary<string, string> queryParams)
        {
            Uri uriRedirect = new Uri(QueryHelpers.AddQueryString(baseUrl, queryParams));
            return Redirect(uriRedirect.AbsoluteUri);
        }

        private CoreException ConvertToCdpException(DataNotFoundException ex)
        {
            CoreException coreException;

            if (!string.IsNullOrEmpty(ex.Message)
                && string.IsNullOrEmpty(ex.SystemLabel)
                && string.IsNullOrEmpty(ex.Message_En)
                && string.IsNullOrEmpty(ex.Message_Th))
            {
                // ex.Message เป็น message สำหรับ internal for logging ฉะนั้นจะไม่ response to frontend
                coreException = _common_Ex_Factory.Build(1000404);
            }
            else
            {
                coreException = _common_Ex_Factory.Build(1000404,
                    systemLabel: ex.SystemLabel,
                    message: "", // ex.Message เป็น message สำหรับ internal for logging ฉะนั้นจะไม่ response to frontend
                    title_en: "",
                    message_en: ex.Message_En,
                    title_th: "",
                    message_th: ex.Message_Th,
                    overrideIfNotEmpty: true
                );
            }

            return coreException;
        }

        private CoreException ConvertToCdpException(BL_Exception ex)
        {
            CoreException coreException;

            if (!string.IsNullOrEmpty(ex.Message)
                && string.IsNullOrEmpty(ex.SystemLabel)
                && string.IsNullOrEmpty(ex.Message_En)
                && string.IsNullOrEmpty(ex.Message_Th))
            {
                // ex.Message เป็น message สำหรับ internal for logging ฉะนั้นจะไม่ response to frontend
                coreException = _common_Ex_Factory.Build(1000422);
            }
            else
            {
                coreException = _common_Ex_Factory.Build(1000422,
                    systemLabel: ex.SystemLabel,
                    message: "", // ex.Message เป็น message สำหรับ internal for logging ฉะนั้นจะไม่ response to frontend
                    title_en: "",
                    message_en: ex.Message_En,
                    title_th: "",
                    message_th: ex.Message_Th,
                    overrideIfNotEmpty: true
                );
            }

            return coreException;
        }

        private CoreException ConvertToCdpException(BadRequestException ex)
        {
            CoreException coreException;

            string message = "";
            if (string.IsNullOrWhiteSpace(ex.Message_En))
                message = ex.Message_En;
            if (string.IsNullOrWhiteSpace(ex.Message_Th))
                message = ex.Message_Th;

            coreException = _common_Ex_Factory.Build(1000400,
                systemLabel: ex.SystemLabel,
                message: message,
                title_en: "",
                message_en: ex.Message_En,
                title_th: "",
                message_th: ex.Message_Th,
                overrideIfNotEmpty: true
            );

            Dictionary<string, string> dictParamMessages = ex.ParamMessages;
            foreach (var item in dictParamMessages)
            {
                if (coreException.ApiError.Validate == null)
                    coreException.ApiError.Validate = new Validate_Errors();

                coreException.ApiError.Validate?.Fields.Add(new Field_Error()
                {
                    Key = item.Key, // paramName 
                    Message = item.Value // Message
                });
            }

            return coreException;
        }

        private CoreException ConvertToCdpException(AppException ex)
        {
            CoreException coreException = _common_Ex_Factory.Build(1000422);
            return coreException;
        }
    }
}