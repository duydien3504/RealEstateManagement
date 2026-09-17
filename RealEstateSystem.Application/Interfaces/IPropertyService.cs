using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<CreatePropertyResponse> CreatePropertyAsync(Guid userId, CreatePropertyRequest request, CancellationToken cancellationToken);
        Task<List<PropertyResponseDto>> GetMyPropertiesAsync(Guid ownerId, CancellationToken cancellationToken);
        Task<PropertyResponseDto?> GetPropertyDetailAsync(Guid propertyId, CancellationToken cancellationToken);
        Task<PaginatedPropertiesResponse> GetPublicPropertiesAsync(GetPublicPropertiesRequest request, CancellationToken cancellationToken);
        Task<bool> UpdatePropertyAsync(Guid userId, Guid propertyId, UpdatePropertyRequest request, CancellationToken cancellationToken);
        Task<bool> SoftDeletePropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
        Task<bool> ApprovePropertyAsync(Guid propertyId, CancellationToken cancellationToken);
        Task<bool> RejectPropertyAsync(Guid propertyId, CancellationToken cancellationToken);
        Task<PropertyActionResponse> ExtendPropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
        Task<PropertyActionResponse> UpgradeToPremiumAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
    }
}
