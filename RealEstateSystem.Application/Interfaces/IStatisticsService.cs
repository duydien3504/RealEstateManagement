using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<DashboardOverviewResponse> GetDashboardOverviewAsync(CancellationToken cancellationToken);
        Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync(CancellationToken cancellationToken);
        Task<List<MonthlyListingsResponse>> GetMonthlyListingsAsync(CancellationToken cancellationToken);
        Task<byte[]> ExportReportToExcelAsync(string reportType, CancellationToken cancellationToken);
    }
}
