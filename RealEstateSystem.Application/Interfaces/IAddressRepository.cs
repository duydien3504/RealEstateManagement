using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAddressRepository
    {
        Task ClearAllAsync(CancellationToken cancellationToken);
        Task SaveProvincesAndWardsAsync(List<Province> provinces, CancellationToken cancellationToken);
        Task<List<Province>> GetProvincesAsync(CancellationToken cancellationToken);
        Task<List<Ward>> GetWardsByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken);
    }
}
