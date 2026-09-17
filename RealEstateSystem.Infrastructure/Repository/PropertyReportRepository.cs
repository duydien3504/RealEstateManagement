using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class PropertyReportRepository : IPropertyReportRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PropertyReport report, CancellationToken cancellationToken)
        {
            await _context.PropertyReports.AddAsync(report, cancellationToken);
        }

        public async Task<bool> HasUserReportedPropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            return await _context.PropertyReports.AnyAsync(r => r.UserId == userId && r.PropertyId == propertyId, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
