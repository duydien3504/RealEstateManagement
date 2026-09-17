using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken);
        Task<int> GetTotalPropertiesCountAsync(CancellationToken cancellationToken);
        Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken);
        Task<int> GetTotalPremiumPropertiesCountAsync(CancellationToken cancellationToken);
        Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync(CancellationToken cancellationToken);
        Task<List<MonthlyListingsResponse>> GetMonthlyListingsAsync(CancellationToken cancellationToken);
        Task<List<TransactionReportDto>> GetTransactionReportDataAsync(CancellationToken cancellationToken);
        Task<List<PropertyReportDto>> GetPropertyReportDataAsync(CancellationToken cancellationToken);
    }
}
