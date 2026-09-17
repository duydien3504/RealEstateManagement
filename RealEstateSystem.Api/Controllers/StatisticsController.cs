using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/statistics")]
    [Authorize(Roles = "Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
        {
            var result = await _statisticsService.GetDashboardOverviewAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue(CancellationToken cancellationToken)
        {
            var result = await _statisticsService.GetMonthlyRevenueAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("monthly-listings")]
        public async Task<IActionResult> GetMonthlyListings(CancellationToken cancellationToken)
        {
            var result = await _statisticsService.GetMonthlyListingsAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportReport([FromQuery] string reportType, CancellationToken cancellationToken)
        {
            var fileBytes = await _statisticsService.ExportReportToExcelAsync(reportType, cancellationToken);
            var fileName = $"{reportType.ToLower()}_report_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
