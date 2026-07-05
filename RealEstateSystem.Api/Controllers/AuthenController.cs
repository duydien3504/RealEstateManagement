using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;

        public AuthenController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authenService.RegisterAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình xử lý đăng ký." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var result = await _authenService.LoginAsync(request, ipAddress, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                return Unauthorized(new { Message = exception.Message });
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình xử lý đăng nhập." });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, [FromQuery] string otp, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authenService.VerifyOtpAsync(request, otp, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình xác thực OTP." });
            }
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authenService.ForgetPasswordAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình xử lý yêu cầu quên mật khẩu." });
            }
        }

        [HttpPost("verify-change-password")]
        public async Task<IActionResult> VerifyChangePassword([FromBody] VerifyChangePasswordRequest request, [FromQuery] string otp, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authenService.VerifyChangePasswordAsync(request, otp, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình xác thực OTP đổi mật khẩu." });
            }
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub) 
                                  ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { Message = "Yêu cầu xác thực không hợp lệ." });
                }

                var result = await _authenService.ChangePasswordAsync(userId, request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình đổi mật khẩu." });
            }
        }
    }
}
