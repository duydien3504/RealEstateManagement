using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IWalletService _walletService;

        public ProfileController(IProfileService profileService, IWalletService walletService)
        {
            _profileService = profileService;
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _profileService.GetProfileAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile([FromBody] DeleteProfileRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _profileService.DeleteProfileAsync(userId, request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Vui lòng chọn một file ảnh hợp lệ." });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { Message = "Kích thước ảnh phải nhỏ hơn 5MB." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                return BadRequest(new { Message = "Chỉ chấp nhận file ảnh định dạng JPG hoặc PNG." });
            }

            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var emailClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)
                             ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            var result = await _profileService.UploadAvatarAsync(userId, emailClaim.Value, fileBytes, file.FileName, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("up-role")]
        public async Task<IActionResult> RegisterUpRoleOwner([FromBody] UpRoleRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var result = await _profileService.RegisterUpRoleOwnerAsync(userId, request, ipAddress, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("/api/payment/vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayReturn(CancellationToken cancellationToken)
        {
            var queryParams = HttpContext.Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());

            if (queryParams.TryGetValue("vnp_TxnRef", out var txnRef) && txnRef.StartsWith("WT_", StringComparison.Ordinal))
            {
                var walletResult = await _walletService.ProcessReturnUrlAsync(queryParams, cancellationToken);
                return Ok(walletResult);
            }

            return BadRequest(new { Message = "Yêu cầu thanh toán không hợp lệ." });
        }

        [HttpGet("/api/payment/vnpay-ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayIpn(CancellationToken cancellationToken)
        {
            var queryParams = HttpContext.Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());

            if (queryParams.TryGetValue("vnp_TxnRef", out var txnRef) && txnRef.StartsWith("WT_", StringComparison.Ordinal))
            {
                var walletResult = await _walletService.ProcessIpnAsync(queryParams, cancellationToken);
                return Ok(walletResult);
            }

            return BadRequest(new { Message = "Yêu cầu thanh toán không hợp lệ." });
        }
    }
}
