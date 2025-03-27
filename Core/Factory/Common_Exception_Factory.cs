using Core.Api.Model;
using Core.Exceptions;
using Core.Utils;

namespace Core.Factory
{
    /// <summary>
    /// CDP_Exception_Factory เป็น class สร้าง CoreException ที่มี business logic ของ BCRM Platform ปนอยู่
    /// จึงทำให้ถ้าต้องการ reuse implementation ที่เป็น concept เดียวกันกับ BCRM Platform ใน project อื่นที่ไม่ใช่ BCRM Platform ไม่สามารถทำได้
    /// จึงมี class นี้ขึ้นมาเพื่อตอบโจทย์ความเป็นกลางในทุกๆ project ที่ต้องการใช้ exception concept เดียวกับ BCRM Platform
    /// แต่ไม่ต้องการให้มี business logic ของ BCRM Platform ปนอยู่ <br/><br/>
    /// 
    /// business logic ของ BCRM Platform เช่น <br/>
    /// - connect DB ของ BCRM Platform <br/>
    /// - มี keyword BCRM Platform อยู่ใน Exception message
    /// </summary>
    public class Common_Exception_Factory : ICommon_Exception_Factory
    {
        private List<Api_ErrorCode> _listApiErrorCodes = new List<Api_ErrorCode>(); 

        public CoreException Build(long errorId, string message = "")
        {
            Api_ErrorCode apiError = Get_Error(errorId);
            if (apiError == null) throw new DataNotFoundException($"Not found ErrorId = {errorId}");

            CoreException ex = new CoreException(apiError, message);
            return ex;
        }

        public CoreException Build(long errorId, string message_en, string message_th)
        {
            CoreException ex = Build(errorId, message_en);

            if (!string.IsNullOrWhiteSpace(message_en))
                ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Message = message_en;

            if (!string.IsNullOrWhiteSpace(message_th))
                ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Message = message_th;

            return ex;
        }

        public CoreException Build(long errorId, string message, string message_en, string message_th)
        {
            CoreException ex = Build(errorId, message);

            if (!string.IsNullOrWhiteSpace(message_en))
                ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Message = message_en;

            if (!string.IsNullOrWhiteSpace(message_th))
                ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Message = message_th;

            return ex;
        }

        public CoreException Build(long errorId, string title_en, string message_en, string title_th, string message_th)
        {
            CoreException ex = Build(errorId, message_en);

            ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Title = title_en;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Message = message_en;

            ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Title = title_th;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Message = message_th;

            return ex;
        }

        public CoreException Build(long errorId, string message, string title_en, string message_en, string title_th, string message_th)
        {
            CoreException ex = Build(errorId, message);

            ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Title = title_en;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Message = message_en;

            ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Title = title_th;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Message = message_th;

            return ex;
        }

        public CoreException Build(long errorId, string systemLabel, string message, string title_en, string message_en, string title_th, string message_th)
        {
            CoreException ex = Build(errorId, message);

            ex.ApiError.System = systemLabel;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Title = title_en;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Message = message_en;

            ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Title = title_th;
            ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Message = message_th;

            return ex;
        }

        public CoreException Build(long errorId, string systemLabel, string message, string title_en, string message_en, string title_th, string message_th, bool overrideIfNotEmpty)
        {
            if (overrideIfNotEmpty)
            {
                CoreException ex = Build(errorId, message);

                if (!string.IsNullOrWhiteSpace(systemLabel))
                    ex.ApiError.System = systemLabel;

                if (!string.IsNullOrWhiteSpace(title_en))
                    ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Title = title_en;
                if (!string.IsNullOrWhiteSpace(message_en))
                    ex.ApiError.Locale[Api_Error.LOCALE_KEY_EN].Message = message_en;

                if (!string.IsNullOrWhiteSpace(title_th))
                    ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Title = title_th;
                if (!string.IsNullOrWhiteSpace(message_th))
                    ex.ApiError.Locale[Api_Error.LOCALE_KEY_TH].Message = message_th;

                return ex;
            }
            else
            {
                return Build(errorId, systemLabel, message, title_en, message_en, title_th, message_th);
            }
        }

        public CoreException Build(int statusCode, long errorId, string message = "")
        {
            Api_ErrorCode apiError = Get_Error(errorId);
            if (apiError == null) throw new DataNotFoundException($"Not found ErrorId = {errorId}");

            CoreException ex = new CoreException(statusCode, apiError, message);
            return ex;
        }

        public CoreException Build(Exception inner, string message = "")
        {
            int errorId = 1000500;
            Api_ErrorCode apiError = null;

            try
            {
                apiError = Get_Error(errorId);
            }
            catch (Exception ex)
            {
                apiError = GenerateFatalBCRMApiError();
            }

            if (apiError == null) throw new DataNotFoundException($"Not found ErrorId = {errorId}");

            CoreException except = new CoreException(apiError, inner, message);
            return except;
        }

        public CoreException Build(long errorId, Exception inner, string message = "")
        {
            Api_ErrorCode apiError = Get_Error(errorId);
            if (apiError == null) throw new DataNotFoundException($"Not found ErrorId = {errorId}");

            CoreException except = new CoreException(apiError, inner, message);
            return except;
        }

        public CoreException Build(string resourceKey)
        {
            string httpStatusCodeStr = ResourceUtil.GetHttpStatusCode(resourceKey);
            int httpStatusCode = -1;
            if (!int.TryParse(httpStatusCodeStr, out httpStatusCode))
                httpStatusCode = 422;

            Api_ErrorCode api_ErrorCode = new Api_ErrorCode();
            api_ErrorCode.ErrorId = ResourceUtil.GetErrorCode(resourceKey);
            api_ErrorCode.Default_HTTP_Status = httpStatusCode;
            api_ErrorCode.Action = ResourceUtil.GetMessage(resourceKey);
            api_ErrorCode.System_Label = api_ErrorCode.Action;
            api_ErrorCode.Error_Title_En = ResourceUtil.GetTitle(resourceKey, "En");
            api_ErrorCode.Error_Title_Th = ResourceUtil.GetTitle(resourceKey, "Th");
            api_ErrorCode.Error_Message_En = ResourceUtil.GetMessageDetail(resourceKey, "En");
            api_ErrorCode.Error_Message_Th = ResourceUtil.GetMessageDetail(resourceKey, "Th");

            CoreException except = new CoreException(api_ErrorCode);
            return except;
        }

        /// <summary>
        /// วัตถุประสงค์ตั้งต้นดูที่ function <see cref="CDP_Exception_Factory.GenerateFatalBCRMApiError"/> <br/>
        /// แต่สำหรับ class นี้ที่ยังต้องมีเพราะต้องให้ compatible กับ code บางจุดที่มีการ call function นี้
        /// </summary>
        /// <returns></returns>
        public Api_ErrorCode GenerateFatalBCRMApiError()
        {
            Api_ErrorCode apiError = new Api_ErrorCode()
            {
                ErrorId = 1000500,
                System_Label = "Internal Server Error (Fatal)",
                Default_HTTP_Status = 500,
                Error_Title_En = "Internal Server Error",
                Error_Message_En = "Internal Server Error",
                Error_Title_Th = "เกิดปัญหาขัดข้อง",
                Error_Message_Th = "เกิดปัญหาขัดข้อง",
            };
            return apiError;
        }

        public CoreException GenerateFatalBCRMException(string message = "")
        {
            Api_ErrorCode apiError = GenerateFatalBCRMApiError();
            CoreException ex = new CoreException(apiError, message);
            return ex;
        }

        public CoreException GenerateFatalBCRMException(Exception inner, string message = "")
        {
            Api_ErrorCode apiError = GenerateFatalBCRMApiError();
            CoreException ex = new CoreException(apiError, inner, message);
            return ex;
        }

        private Api_ErrorCode Get_Error(long errorId)
        {
            LazyInitApiErrorCodeData();

            Api_ErrorCode apiError = _listApiErrorCodes.Where(x => x.ErrorId == errorId).FirstOrDefault();

            return apiError;
        }

        private void LazyInitApiErrorCodeData()
        {
            if (_listApiErrorCodes.Count == 0)
            {
                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000400,
                    System_Label = "Bad Request",
                    Default_HTTP_Status = 400,
                    Error_Title_En = "Invalid Request",
                    Error_Message_En = "Invalid/Bad request parameters, please verify",
                    Error_Title_Th = "ข้อมูลไม่ถูกต้อง",
                    Error_Message_Th = "คำขอผิดรูปแบบหรือไม่ต้อง กรุณาตรวจสอบ",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000401,
                    System_Label = "Unauthorized",
                    Default_HTTP_Status = 401,
                    Error_Title_En = "Unauthorized",
                    Error_Message_En = "Unauthorized, please verify your Identity",
                    Error_Title_Th = "ไม่มีสิทธิ์ในการทำรายการ",
                    Error_Message_Th = "ไม่มีสิทธิ์ในการทำรายการ กรุณาระบุตัวตน",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000403,
                    System_Label = "Permission Denied",
                    Default_HTTP_Status = 403,
                    Error_Title_En = "Permission Denied",
                    Error_Message_En = "Unable to proceed with transaction, please verify",
                    Error_Title_Th = "ไม่มีสิทธิ์ในการทำรายการ",
                    Error_Message_Th = "ไม่มีสิทธิ์ในการทำรายการ กรุณาตรวจสอบ",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000404,
                    System_Label = "Not Found",
                    Default_HTTP_Status = 404,
                    Error_Title_En = "Not Found",
                    Error_Message_En = "Resource/Transaction not found",
                    Error_Title_Th = "ไม่พบข้อมูล",
                    Error_Message_Th = "ไม่พบข้อมูลหรือรายการที่ระบุ",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000405,
                    System_Label = "Method not Allowed",
                    Default_HTTP_Status = 405,
                    Error_Title_En = "Http Method not allowed",
                    Error_Message_En = "Http Method now allowed, please verify",
                    Error_Title_Th = "วิธีการร้องขอไม่ถูกต้อง",
                    Error_Message_Th = "วิธีการร้องขอในการทำรายไม่ถูกต้อง กรุณาตรวจสอบ",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000409,
                    System_Label = "Conflict with current state/data",
                    Default_HTTP_Status = 409,
                    Error_Title_En = "Request conflict",
                    Error_Message_En = "Invalid request, unable to process due to conflict with current state/data",
                    Error_Title_Th = "วิธีการร้องขอไม่ถูกต้อง",
                    Error_Message_Th = "วิธีการร้องขอในการทำรายไม่ถูกต้อง กรุณาตรวจสอบ",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000422,
                    System_Label = "Unprocessable",
                    Default_HTTP_Status = 422,
                    Error_Title_En = "Unprocessable",
                    Error_Message_En = "Can not process the request due to invalid data state",
                    Error_Title_Th = "สถานะไม่ถูกต้อง",
                    Error_Message_Th = "สถานะของรายการหรือข้อมูลไม่ถูกต้อง",
                });

                _listApiErrorCodes.Add(new Api_ErrorCode()
                {
                    ErrorId = 1000500,
                    System_Label = "Internal Server Error",
                    Default_HTTP_Status = 500,
                    Error_Title_En = "Internal Server Error",
                    Error_Message_En = "Internal Server Error",
                    Error_Title_Th = "เกิดปัญหาขัดข้อง",
                    Error_Message_Th = "เกิดปัญหาขัดข้อง",
                });
            }
        }
    }
}
