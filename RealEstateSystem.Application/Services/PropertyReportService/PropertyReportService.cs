using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.PropertyReportService
{
    public class PropertyReportService : IPropertyReportService
    {
        private readonly IPropertyReportRepository _propertyReportRepository;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyReportService(IPropertyReportRepository propertyReportRepository, IPropertyRepository propertyRepository)
        {
            _propertyReportRepository = propertyReportRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<PropertyReportResponse> CreateReportAsync(Guid userId, CreatePropertyReportRequest request, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
            {
                throw new NotFoundException("Tin đăng không tồn tại.");
            }

            var hasReported = await _propertyReportRepository.HasUserReportedPropertyAsync(userId, request.PropertyId, cancellationToken);
            if (hasReported)
            {
                throw new BusinessRuleException("Bạn đã báo cáo tin đăng này rồi.");
            }

            var report = new PropertyReport
            {
                ReportId = Guid.NewGuid(),
                UserId = userId,
                PropertyId = request.PropertyId,
                Reason = request.Reason,
                Details = request.Details,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _propertyReportRepository.AddAsync(report, cancellationToken);
            await _propertyReportRepository.SaveChangesAsync(cancellationToken);

            return new PropertyReportResponse
            {
                ReportId = report.ReportId,
                UserId = report.UserId,
                PropertyId = report.PropertyId,
                Reason = report.Reason,
                Details = report.Details,
                Status = report.Status.ToString(),
                CreatedAt = report.CreatedAt
            };
        }
    }
}
