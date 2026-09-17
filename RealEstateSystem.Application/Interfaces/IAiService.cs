using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAiService
    {
        Task<OptimizeListingResponse> OptimizeListingAsync(OptimizeListingRequest request, CancellationToken cancellationToken);
    }
}
