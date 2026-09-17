using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
        Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken);
        Task<DeleteProfileResponse> DeleteProfileAsync(Guid userId, DeleteProfileRequest request, CancellationToken cancellationToken);
        Task<UploadAvatarResponse> UploadAvatarAsync(Guid userId, string tokenEmail, byte[] fileBytes, string fileName, CancellationToken cancellationToken);
        Task<UpRoleResponse> RegisterUpRoleOwnerAsync(Guid userId, UpRoleRequest request, string ipAddress, CancellationToken cancellationToken);
    }
}
