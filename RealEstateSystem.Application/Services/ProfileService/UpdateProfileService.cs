using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class UpdateProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateProfileService> _logger;

        public UpdateProfileService(IUserRepository userRepository, ILogger<UpdateProfileService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Cập nhật thông tin thất bại: Tài khoản với ID {UserId} không tồn tại.", userId);
                throw new NotFoundException("Tài khoản không tồn tại.");
            }

            var oldFullName = user.FullName;
            user.FullName = request.FullName;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Người dùng {Email} (ID: {UserId}) đã cập nhật FullName từ '{OldName}' thành '{NewName}'.", user.Email, user.UserId, oldFullName, request.FullName);

            var updatedProfile = new UserProfileResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status.ToString(),
                AvatarUrl = user.AvatarUrl,
                RoleName = user.Role.NameRole.ToString(),
                CreatedAt = user.CreatedAt
            };

            return new UpdateProfileResponse
            {
                Message = "Cập nhật thông tin cá nhân thành công.",
                UpdatedProfile = updatedProfile
            };
        }
    }
}

