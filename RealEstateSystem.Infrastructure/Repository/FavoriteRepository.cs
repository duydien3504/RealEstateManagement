using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddFavoriteAsync(UserFavorite favorite, CancellationToken cancellationToken)
        {
            await _context.UserFavorites.AddAsync(favorite, cancellationToken);
        }

        public Task RemoveFavoriteAsync(UserFavorite favorite, CancellationToken cancellationToken)
        {
            _context.UserFavorites.Remove(favorite);
            return Task.CompletedTask;
        }

        public async Task<UserFavorite?> GetFavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            return await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId, cancellationToken);
        }

        public async Task<List<Property>> GetFavoritePropertiesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Properties
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Ward)
                    .ThenInclude(w => w.Province)
                .Include(p => p.PropertyMedias)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Where(p => _context.UserFavorites.Any(f => f.UserId == userId && f.PropertyId == p.PropertyId))
                .ToListAsync(cancellationToken);
        }
    }
}
