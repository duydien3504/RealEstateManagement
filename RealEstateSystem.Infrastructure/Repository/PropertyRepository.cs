using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Property property, CancellationToken cancellationToken)
        {
            await _context.Properties.AddAsync(property, cancellationToken);
        }

        public async Task AddPropertyPaymentAsync(PropertyPayment propertyPayment, CancellationToken cancellationToken)
        {
            await _context.PropertyPayments.AddAsync(propertyPayment, cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.CommitAsync(cancellationToken);
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.RollbackAsync(cancellationToken);
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
        {
            return await _context.Properties
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Ward)
                    .ThenInclude(w => w.Province)
                .Include(p => p.PropertyMedias)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Property?> GetPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            return await _context.Properties
                .Include(p => p.Category)
                .Include(p => p.Ward)
                    .ThenInclude(w => w.Province)
                .Include(p => p.PropertyMedias)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId, cancellationToken);
        }

        public async Task<Property?> GetPropertyByIdWithDeletedAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            return await _context.Properties
                .IgnoreQueryFilters()
                .Include(p => p.Category)
                .Include(p => p.Ward)
                    .ThenInclude(w => w.Province)
                .Include(p => p.PropertyMedias)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId, cancellationToken);
        }

        public async Task<(List<Property> Items, int TotalCount)> GetPublicPropertiesAsync(GetPublicPropertiesRequest request, CancellationToken cancellationToken)
        {
            var query = _context.Properties
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Ward)
                    .ThenInclude(w => w.Province)
                .Include(p => p.PropertyMedias)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Where(p => p.DisplayStatusValue == DisplayStatus.Approved && !p.IsDeleted)
                .Where(p => p.ExpiredAt == null || p.ExpiredAt > DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(p => EF.Functions.Collate(p.Title, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI"))
                                         || (p.Description != null && EF.Functions.Collate(p.Description, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI")))
                                         || (p.AddressDetail != null && EF.Functions.Collate(p.AddressDetail, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI"))));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (request.WardId.HasValue)
            {
                query = query.Where(p => p.WardId == request.WardId.Value);
            }
            else if (request.ProvinceId.HasValue)
            {
                query = query.Where(p => p.Ward.ProvinceId == request.ProvinceId.Value);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }

            if (request.MinArea.HasValue)
            {
                query = query.Where(p => p.Area >= request.MinArea.Value);
            }

            if (request.MaxArea.HasValue)
            {
                query = query.Where(p => p.Area <= request.MaxArea.Value);
            }

            if (request.NumBedrooms.HasValue)
            {
                query = query.Where(p => p.NumBedrooms == request.NumBedrooms.Value);
            }

            if (request.IsPremium.HasValue)
            {
                query = query.Where(p => p.IsPremium == request.IsPremium.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var sortedQuery = query.OrderByDescending(p => p.IsPremium);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                sortedQuery = request.SortBy.ToLower() switch
                {
                    "price_asc" => sortedQuery.ThenBy(p => p.Price),
                    "price_desc" => sortedQuery.ThenByDescending(p => p.Price),
                    "area_asc" => sortedQuery.ThenBy(p => p.Area),
                    "area_desc" => sortedQuery.ThenByDescending(p => p.Area),
                    "newest" => sortedQuery.ThenByDescending(p => p.CreatedAt),
                    _ => sortedQuery.ThenByDescending(p => p.CreatedAt)
                };
            }
            else
            {
                sortedQuery = sortedQuery.ThenByDescending(p => p.CreatedAt);
            }

            var items = await sortedQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public Task UpdateAsync(Property property, CancellationToken cancellationToken)
        {
            var entry = _context.Entry(property);
            if (entry.State == EntityState.Detached)
            {
                _context.Properties.Update(property);
            }
            return Task.CompletedTask;
        }

        public async Task DeletePropertyAmenitiesAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            var amenities = await _context.PropertyAmenities.Where(pa => pa.PropertyId == propertyId).ToListAsync(cancellationToken);
            _context.PropertyAmenities.RemoveRange(amenities);
        }

        public async Task DeletePropertyMediasAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            var medias = await _context.PropertyMedias.Where(pm => pm.PropertyId == propertyId).ToListAsync(cancellationToken);
            _context.PropertyMedias.RemoveRange(medias);
        }
    }
}
