using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAmenityService
    {
        Task<List<AmenityResponseDto>> GetAmenitiesAsync(CancellationToken cancellationToken);
    }
}
