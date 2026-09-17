using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IPropertyReportService
    {
        Task<PropertyReportResponse> CreateReportAsync(Guid userId, CreatePropertyReportRequest request, CancellationToken cancellationToken);
    }
}
