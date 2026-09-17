using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenityService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<List<AmenityResponseDto>> GetAmenitiesAsync(CancellationToken cancellationToken)
        {
            var amenities = await _amenityRepository.GetAllAsync(cancellationToken);
            return amenities.Select(a => new AmenityResponseDto
            {
                AmenityId = a.AmenityId,
                Name = a.Name,
                IconUrl = a.IconUrl
            })
            .OrderBy(a => a.Name)
            .ToList();
        }
    }
}
