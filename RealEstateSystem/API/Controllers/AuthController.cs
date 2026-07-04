using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.API.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                var tokenResponse = await _authService.LoginAsync(request);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, cookieOptions);
                
                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công",
                    AccessToken = tokenResponse.AccessToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }
}
