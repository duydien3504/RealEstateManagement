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
        Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
        Task AddOwnerUpgradePaymentAsync(OwnerUpgradePayment ownerUpgradePayment, CancellationToken cancellationToken);
        Task<Transaction?> GetTransactionByCodeAsync(string transactionCode, CancellationToken cancellationToken);
        Task<OwnerProfileRequest?> GetLatestPendingOwnerProfileRequestAsync(Guid userId, CancellationToken cancellationToken);
        Task AddOwnerProfileRequestAsync(OwnerProfileRequest request, CancellationToken cancellationToken);
        Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
