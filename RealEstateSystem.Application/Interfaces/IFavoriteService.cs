using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<ToggleFavoriteResponse> ToggleFavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
        Task<bool> UnfavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
        Task<List<PropertyResponseDto>> GetFavoritePropertiesAsync(Guid userId, CancellationToken cancellationToken);
    }
}
