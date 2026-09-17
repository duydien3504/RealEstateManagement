using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public StatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.CountAsync(u => !u.IsDeleted, cancellationToken);
        }

        public async Task<int> GetTotalPropertiesCountAsync(CancellationToken cancellationToken)
        {
            return await _context.Properties.CountAsync(p => !p.IsDeleted, cancellationToken);
        }

        public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Where(t => t.PaymentStatus == PaymentStatus.Success)
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<int> GetTotalPremiumPropertiesCountAsync(CancellationToken cancellationToken)
        {
            return await _context.Properties.CountAsync(p => p.IsPremium && !p.IsDeleted, cancellationToken);
        }

        public async Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync(CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Where(t => t.PaymentStatus == PaymentStatus.Success && t.PaidAt != null)
                .GroupBy(t => new { Year = t.PaidAt!.Value.Year, Month = t.PaidAt!.Value.Month })
                .Select(g => new MonthlyRevenueResponse
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(t => t.Amount)
                })
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.Month)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<MonthlyListingsResponse>> GetMonthlyListingsAsync(CancellationToken cancellationToken)
        {
            return await _context.Properties
                .Where(p => !p.IsDeleted)
                .GroupBy(p => new { Year = p.CreatedAt.Year, Month = p.CreatedAt.Month })
                .Select(g => new MonthlyListingsResponse
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.Month)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TransactionReportDto>> GetTransactionReportDataAsync(CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Include(t => t.User)
                .Select(t => new TransactionReportDto
                {
                    TransactionId = t.TransactionId,
                    Email = t.User.Email,
                    Amount = t.Amount,
                    TransactionCode = t.TransactionCode,
                    PaymentStatus = t.PaymentStatus.ToString(),
                    PaidAt = t.PaidAt
                })
                .OrderByDescending(t => t.PaidAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PropertyReportDto>> GetPropertyReportDataAsync(CancellationToken cancellationToken)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Select(p => new PropertyReportDto
                {
                    PropertyId = p.PropertyId,
                    Title = p.Title,
                    OwnerEmail = p.Owner.Email,
                    Price = p.Price,
                    Area = p.Area,
                    DisplayStatus = p.DisplayStatusValue.ToString(),
                    CreatedAt = p.CreatedAt
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
