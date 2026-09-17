using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAddressService
    {
        Task SyncProvincesAndWardsAsync(CancellationToken cancellationToken);
        Task<List<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken);
        Task<List<WardDto>> GetWardsByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken);
    }
}
