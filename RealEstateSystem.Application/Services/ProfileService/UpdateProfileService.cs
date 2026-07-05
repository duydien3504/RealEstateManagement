using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class UpdateProfileService
    {
        private readonly IUserRepository _userRepository;

        public UpdateProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại.");
            }

            user.FullName = request.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

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
