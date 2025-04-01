using AuthService.Models;
using AuthService.Services;
using Core;
using Microsoft.AspNetCore.Mvc;
using Core.Api.Controller;
using Core.Constant;
using Core.Utils;

namespace Auth.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        // public AuthController(
        //     ILogger<AuthController> logger,
        //     ICommon_Exception_Factory common_Ex_Factory,
        //     IHttpContextAccessor httpContext_Accessor, ControllerUtil ctrlUtil, ILogger logger1,
        //     ICommon_Exception_Factory commonExFactory, IHttpContextAccessor httpContextAccessor) : base(logger,
        //     common_Ex_Factory, httpContext_Accessor)
        // {
        //     _ctrl_Util = ctrlUtil;
        //     _logger = logger1;
        //     _common_Ex_Factory = commonExFactory;
        //     _httpContext_Accessor = httpContextAccessor;
        //     // // Services
        //     // _httpContext_Accessor = httpContext_Accessor ?? throw new ArgumentNullException(nameof(httpContext_Accessor));
        //     // _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //     // _common_Ex_Factory = common_Ex_Factory ?? throw new ArgumentNullException(nameof(common_Ex_Factory));
        //     //
        //     // // Util
        //     // _ctrl_Util = new ControllerUtil(this, _httpContext_Accessor);
        // }

        public AuthController(
            ILogger<AuthController> logger,
            ICommon_Exception_Factory commonExFactory,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService
        )
            : base(logger, commonExFactory, httpContextAccessor)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginRequest req)
        {
            try
            {
                var result = await _userService.Login(req);
                Data = new
                {
                    result
                };
                Status = CoreConstant.Api.Result_Status.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ApiException = e;
            }

            // Data ={}
            return Build_JsonResp();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterRequest req)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(req);
                Data = new
                {
                    result
                };
                Status = CoreConstant.Api.Result_Status.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ApiException = e;
            }

            // Data ={}
            return Build_JsonResp();
        }

        [HttpGet]
        public async Task<IActionResult> Test(LoginRequest req)
        {
            return Ok("test");
        }
    }
}