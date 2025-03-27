using Core;
using Microsoft.AspNetCore.Mvc;
using Core.Api.Controller;
using Core.Utils;

namespace Auth.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthControllerApi : BaseApiController
    {
        public Exception ApiException { get; set; } = null;

        // Util
        public readonly ControllerUtil _ctrl_Util;
        public readonly ILogger _logger;
        public readonly ICommon_Exception_Factory _common_Ex_Factory;
        public readonly IHttpContextAccessor _httpContext_Accessor;

        public AuthControllerApi(ILogger<AuthControllerApi> logger,
            ICommon_Exception_Factory common_Ex_Factory,
            IHttpContextAccessor httpContext_Accessor) : base(logger, common_Ex_Factory, httpContext_Accessor)
        {
            // Services
            _httpContext_Accessor =
                httpContext_Accessor ?? throw new ArgumentNullException(nameof(httpContext_Accessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _common_Ex_Factory = common_Ex_Factory ?? throw new ArgumentNullException(nameof(common_Ex_Factory));

            // Util
            _ctrl_Util = new ControllerUtil(this, _httpContext_Accessor);
        }

        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            try
            {
                Data = new
                {
                    username = username
                };
                Status = "200";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ApiException = e;
            }

            // Data ={}
            return Build_JsonResp();
        }
    }
}