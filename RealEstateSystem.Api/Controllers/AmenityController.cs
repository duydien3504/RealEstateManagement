using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityService _amenityService;

        public AmenityController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAmenities(CancellationToken cancellationToken)
        {
            var result = await _amenityService.GetAmenitiesAsync(cancellationToken);
            return Ok(result);
        }
    }
}
