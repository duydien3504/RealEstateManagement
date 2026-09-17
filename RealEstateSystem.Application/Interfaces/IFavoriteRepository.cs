using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IFavoriteRepository
    {
        Task AddFavoriteAsync(UserFavorite favorite, CancellationToken cancellationToken);
        Task RemoveFavoriteAsync(UserFavorite favorite, CancellationToken cancellationToken);
        Task<UserFavorite?> GetFavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken);
        Task<List<Property>> GetFavoritePropertiesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
