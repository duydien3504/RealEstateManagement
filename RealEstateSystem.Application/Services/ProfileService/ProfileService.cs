using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class ProfileService : IProfileService
    {
        private readonly GetProfileService _getProfileService;
        private readonly UpdateProfileService _updateProfileService;
        private readonly DeleteProfileService _deleteProfileService;
        private readonly UploadAvatarService _uploadAvatarService;
        private readonly UpRoleService _upRoleService;

        public ProfileService(
            GetProfileService getProfileService,
            UpdateProfileService updateProfileService,
            DeleteProfileService deleteProfileService,
            UploadAvatarService uploadAvatarService,
            UpRoleService upRoleService)
        {
            _getProfileService = getProfileService;
            _updateProfileService = updateProfileService;
            _deleteProfileService = deleteProfileService;
            _uploadAvatarService = uploadAvatarService;
            _upRoleService = upRoleService;
        }

        public async Task<UserProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _getProfileService.GetProfileAsync(userId, cancellationToken);
        }

        public async Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            return await _updateProfileService.UpdateProfileAsync(userId, request, cancellationToken);
        }

        public async Task<DeleteProfileResponse> DeleteProfileAsync(Guid userId, DeleteProfileRequest request, CancellationToken cancellationToken)
        {
            return await _deleteProfileService.DeleteProfileAsync(userId, request, cancellationToken);
        }

        public async Task<UploadAvatarResponse> UploadAvatarAsync(Guid userId, string tokenEmail, byte[] fileBytes, string fileName, CancellationToken cancellationToken)
        {
            return await _uploadAvatarService.UploadAvatarAsync(userId, tokenEmail, fileBytes, fileName, cancellationToken);
        }

        public async Task<UpRoleResponse> RegisterUpRoleOwnerAsync(Guid userId, UpRoleRequest request, string ipAddress, CancellationToken cancellationToken)
        {
            return await _upRoleService.RegisterUpRoleOwnerAsync(userId, request, ipAddress, cancellationToken);
        }
    }
}
