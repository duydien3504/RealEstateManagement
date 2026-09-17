using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IPropertyRepository
    {
        Task AddAsync(Property property, CancellationToken cancellationToken);
        Task AddPropertyPaymentAsync(PropertyPayment propertyPayment, CancellationToken cancellationToken);
        Task<List<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);
        Task<Property?> GetPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken);
        Task<Property?> GetPropertyByIdWithDeletedAsync(Guid propertyId, CancellationToken cancellationToken);
        Task<(List<Property> Items, int TotalCount)> GetPublicPropertiesAsync(GetPublicPropertiesRequest request, CancellationToken cancellationToken);
        Task UpdateAsync(Property property, CancellationToken cancellationToken);
        Task DeletePropertyAmenitiesAsync(Guid propertyId, CancellationToken cancellationToken);
        Task DeletePropertyMediasAsync(Guid propertyId, CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
