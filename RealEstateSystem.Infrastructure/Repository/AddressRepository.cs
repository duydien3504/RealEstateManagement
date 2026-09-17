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
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ClearAllAsync(CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Wards.RemoveRange(_context.Wards);
                _context.Provinces.RemoveRange(_context.Provinces);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task SaveProvincesAndWardsAsync(List<Province> provinces, CancellationToken cancellationToken)
        {
            await _context.Provinces.AddRangeAsync(provinces, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Province>> GetProvincesAsync(CancellationToken cancellationToken)
        {
            return await _context.Provinces
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Ward>> GetWardsByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken)
        {
            return await _context.Wards
                .AsNoTracking()
                .Where(w => w.ProvinceId == provinceId)
                .ToListAsync(cancellationToken);
        }
    }
}
