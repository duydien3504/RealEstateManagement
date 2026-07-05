using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                                  ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { Message = "Yêu cầu xác thực không hợp lệ." });
                }

                var result = await _profileService.GetProfileAsync(userId, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình lấy thông tin tài khoản." });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                                  ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { Message = "Yêu cầu xác thực không hợp lệ." });
                }

                var result = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình cập nhật thông tin cá nhân." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile([FromBody] DeleteProfileRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                                  ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { Message = "Yêu cầu xác thực không hợp lệ." });
                }

                var result = await _profileService.DeleteProfileAsync(userId, request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống trong quá trình xử lý yêu cầu xóa tài khoản." });
            }
        }
    }
}
