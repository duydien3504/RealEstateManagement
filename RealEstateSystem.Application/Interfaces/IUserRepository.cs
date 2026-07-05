using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CheckExistEmailAsync(string email, CancellationToken cancellationToken);
        Task<Guid?> GetRoleIdByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken);
        Task<User?> GetUserByEmailWithRoleAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<OtpVerification?> GetLatestOtpVerificationWithUserAsync(string email, CancellationToken cancellationToken);
        Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task AddOtpVerificationAsync(OtpVerification otpVerification, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
