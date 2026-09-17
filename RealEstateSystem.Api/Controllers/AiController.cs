using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("optimize")]
        public async Task<IActionResult> OptimizeListing([FromBody] OptimizeListingRequest request, CancellationToken cancellationToken)
        {
            var result = await _aiService.OptimizeListingAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
