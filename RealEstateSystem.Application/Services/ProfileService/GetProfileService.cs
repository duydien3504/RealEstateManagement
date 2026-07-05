using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class GetProfileService
    {
        private readonly IUserRepository _userRepository;

        public GetProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại.");
            }

            return new UserProfileResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status.ToString(),
                AvatarUrl = user.AvatarUrl,
                RoleName = user.Role.NameRole.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
