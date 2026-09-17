using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/properties")]
    [Authorize(Roles = "Admin")]
    public class AdminPropertyController : ControllerBase
    {
        private readonly IAdminPropertyService _adminPropertyService;

        public AdminPropertyController(IAdminPropertyService adminPropertyService)
        {
            _adminPropertyService = adminPropertyService;
        }

        [HttpPost("{propertyId:guid}/restore")]
        public async Task<IActionResult> RestoreProperty(Guid propertyId, CancellationToken cancellationToken)
        {
            var result = await _adminPropertyService.RestoreDeletedPropertyAsync(propertyId, cancellationToken);
            if (!result)
            {
                return BadRequest(new { Message = "Khôi phục tin đăng thất bại." });
            }

            return Ok(new { Message = "Khôi phục tin đăng thành công." });
        }
    }
}
