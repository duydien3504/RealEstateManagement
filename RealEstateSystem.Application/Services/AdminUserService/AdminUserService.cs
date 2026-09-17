using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AdminUserService
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepository;

        public AdminUserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserProfileResponse>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsersAsync(cancellationToken);
            return users.Select(u => new UserProfileResponse
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Status = u.Status.ToString(),
                AvatarUrl = u.AvatarUrl,
                RoleName = u.Role.NameRole.ToString(),
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task<UserProfileResponse> LockUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại.");
            }

            user.Status = StatusType.Block;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

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

        public async Task<UserProfileResponse> UnlockUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại.");
            }

            user.Status = StatusType.Active;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

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

        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại.");
            }

            user.IsDeleted = true;
            user.Status = StatusType.Deleted;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
