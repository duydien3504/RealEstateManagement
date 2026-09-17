using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Services.LocationService
{
    public class AddressService : IAddressService
    {
        private readonly IAddressSyncClient _syncClient;
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressSyncClient syncClient, IAddressRepository addressRepository)
        {
            _syncClient = syncClient;
            _addressRepository = addressRepository;
        }

        public async Task SyncProvincesAndWardsAsync(CancellationToken cancellationToken)
        {
            var rawData = await _syncClient.FetchProvincesAndWardsAsync(cancellationToken);
            if (rawData == null || !rawData.Any())
            {
                return;
            }

            var provinces = new List<Province>();
            foreach (var provDto in rawData)
            {
                var provinceId = Guid.NewGuid();
                var province = new Province
                {
                    ProvinceId = provinceId,
                    Name = provDto.Name
                };

                foreach (var wardDto in provDto.Wards)
                {
                    province.Wards.Add(new Ward
                    {
                        WardId = Guid.NewGuid(),
                        ProvinceId = provinceId,
                        Name = wardDto.Name
                    });
                }

                provinces.Add(province);
            }

            await _addressRepository.ClearAllAsync(cancellationToken);
            await _addressRepository.SaveProvincesAndWardsAsync(provinces, cancellationToken);
        }

        public async Task<List<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken)
        {
            var provinces = await _addressRepository.GetProvincesAsync(cancellationToken);
            return provinces.Select(p => new ProvinceDto
            {
                ProvinceId = p.ProvinceId,
                Name = p.Name
            })
            .OrderBy(p => p.Name)
            .ToList();
        }

        public async Task<List<WardDto>> GetWardsByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken)
        {
            var wards = await _addressRepository.GetWardsByProvinceIdAsync(provinceId, cancellationToken);
            return wards.Select(w => new WardDto
            {
                WardId = w.WardId,
                ProvinceId = w.ProvinceId,
                Name = w.Name
            })
            .OrderBy(w => w.Name)
            .ToList();
        }
    }
}
