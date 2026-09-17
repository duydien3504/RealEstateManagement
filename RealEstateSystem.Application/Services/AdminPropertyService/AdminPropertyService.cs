using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AdminPropertyService
{
    public class AdminPropertyService : IAdminPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public AdminPropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<bool> RestoreDeletedPropertyAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdWithDeletedAsync(propertyId, cancellationToken);
            if (property == null)
            {
                throw new NotFoundException("Tin đăng không tồn tại.");
            }

            if (!property.IsDeleted)
            {
                throw new BusinessRuleException("Tin đăng này chưa bị xóa.");
            }

            property.IsDeleted = false;

            await _propertyRepository.UpdateAsync(property, cancellationToken);
            await _propertyRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
