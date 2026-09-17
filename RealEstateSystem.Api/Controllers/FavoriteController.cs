using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("toggle/{propertyId:guid}")]
        public async Task<IActionResult> ToggleFavorite(Guid propertyId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _favoriteService.ToggleFavoriteAsync(userId, propertyId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("unfavorite/{propertyId:guid}")]
        public async Task<IActionResult> Unfavorite(Guid propertyId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _favoriteService.UnfavoriteAsync(userId, propertyId, cancellationToken);
            if (!result)
            {
                return BadRequest(new { Message = "Tin đăng chưa được lưu hoặc thao tác thất bại." });
            }

            return Ok(new { Message = "Đã bỏ lưu tin đăng bất động sản thành công." });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteProperties(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _favoriteService.GetFavoritePropertiesAsync(userId, cancellationToken);
            return Ok(result);
        }
    }
}
