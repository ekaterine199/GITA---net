using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisterLoginJWTMTO20.Interfaces;
using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.Auth;

namespace RegisterLoginJWTMTO20.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ServiceResponse<int>> Register(UserRegisterDTO userRegisterDTO)
        {
            return await _authService.Register(userRegisterDTO);
        }

        [HttpPost("Login")]
        public async Task<ServiceResponse<string>> Login(UserLoginDTO userLoginDTO)
        {
            return await _authService.Login(userLoginDTO);
        }

        [HttpGet("AdminOnlyPage")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> AdminOnlyPage()
        {
            return new ServiceResponse<string>() { Data = "Access granted"};
        }

        [HttpGet("UserOnlyPage")]
        [Authorize(Roles = "User")]
        public async Task<ServiceResponse<string>> UserOnlyPage()
        {
            return new ServiceResponse<string>() { Data = "Access granted" };
        }
    }
}
