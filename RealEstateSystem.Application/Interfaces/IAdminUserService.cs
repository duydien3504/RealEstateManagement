using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAdminUserService
    {
        Task<List<UserProfileResponse>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<UserProfileResponse> LockUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserProfileResponse> UnlockUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
