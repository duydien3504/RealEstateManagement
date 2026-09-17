using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetCategoriesAsync(cancellationToken);
            return Ok(result);
        }
    }
}
