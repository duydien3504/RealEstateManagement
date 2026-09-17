using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IPropertyReportRepository
    {
        Task AddAsync(PropertyReport report, CancellationToken cancellationToken);
        Task<bool> HasUserReportedPropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
