using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/property-reports")]
    [Authorize]
    public class PropertyReportController : ControllerBase
    {
        private readonly IPropertyReportService _propertyReportService;

        public PropertyReportController(IPropertyReportService propertyReportService)
        {
            _propertyReportService = propertyReportService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreatePropertyReportRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            var result = await _propertyReportService.CreateReportAsync(userId, request, cancellationToken);
            return Ok(result);
        }
    }
}
