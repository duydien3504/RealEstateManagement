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

        public ProfileService(
            GetProfileService getProfileService,
            UpdateProfileService updateProfileService,
            DeleteProfileService deleteProfileService)
        {
            _getProfileService = getProfileService;
            _updateProfileService = updateProfileService;
            _deleteProfileService = deleteProfileService;
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
    }
}

