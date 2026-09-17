namespace RealEstateSystem.Application.Interfaces
{
    public interface IAdminPropertyService
    {
        Task<bool> RestoreDeletedPropertyAsync(Guid propertyId, CancellationToken cancellationToken);
    }
}
