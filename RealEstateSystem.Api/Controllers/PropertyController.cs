using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Owner,Admin")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _propertyService.CreatePropertyAsync(userId, request, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("my-properties")]
        public async Task<IActionResult> GetMyProperties(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _propertyService.GetMyPropertiesAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicProperties([FromQuery] GetPublicPropertiesRequest request, CancellationToken cancellationToken)
        {
            var result = await _propertyService.GetPublicPropertiesAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyDetail(Guid id, CancellationToken cancellationToken)
        {
            var result = await _propertyService.GetPropertyDetailAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound(new { Message = "Không tìm thấy thông tin bất động sản." });
            }

            if (result.DisplayStatus != "Approved")
            {
                var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                                  ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                if (userIdClaim == null || (!isAdmin && (!Guid.TryParse(userIdClaim.Value, out var userId) || result.OwnerId != userId)))
                {
                    return NotFound(new { Message = "Không tìm thấy thông tin bất động sản." });
                }
            }

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var success = await _propertyService.UpdatePropertyAsync(userId, id, request, cancellationToken);
            if (!success)
            {
                return BadRequest(new { Message = "Cập nhật bài đăng thất bại hoặc không có quyền." });
            }

            return Ok(new { Message = "Cập nhật bài đăng bất động sản thành công." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProperty(Guid id, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var success = await _propertyService.SoftDeletePropertyAsync(userId, id, cancellationToken);
            if (!success)
            {
                return BadRequest(new { Message = "Xóa bài đăng thất bại hoặc không có quyền." });
            }

            return Ok(new { Message = "Xóa bài đăng bất động sản thành công." });
        }

        [HttpPost("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveProperty(Guid id, CancellationToken cancellationToken)
        {
            var success = await _propertyService.ApprovePropertyAsync(id, cancellationToken);
            if (!success)
            {
                return BadRequest(new { Message = "Duyệt bài đăng thất bại hoặc không tìm thấy bài viết." });
            }

            return Ok(new { Message = "Duyệt bài đăng bất động sản thành công." });
        }

        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectProperty(Guid id, CancellationToken cancellationToken)
        {
            var success = await _propertyService.RejectPropertyAsync(id, cancellationToken);
            if (!success)
            {
                return BadRequest(new { Message = "Từ chối bài đăng thất bại hoặc không tìm thấy bài viết." });
            }

            return Ok(new { Message = "Từ chối bài đăng bất động sản thành công." });
        }

        [HttpPost("{id:guid}/extend")]
        public async Task<IActionResult> ExtendProperty(Guid id, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _propertyService.ExtendPropertyAsync(userId, id, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("{id:guid}/premium")]
        public async Task<IActionResult> UpgradeToPremium(Guid id, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _propertyService.UpgradeToPremiumAsync(userId, id, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
