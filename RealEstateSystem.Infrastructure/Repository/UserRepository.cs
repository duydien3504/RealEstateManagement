using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckExistEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        }

        public async Task<Guid?> GetRoleIdByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NameRole == roleType, cancellationToken);
            return role?.RoleId;
        }

        public async Task<User?> GetUserByEmailWithRoleAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted, cancellationToken);
        }

        public async Task<OtpVerification?> GetLatestOtpVerificationWithUserAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.OtpVerifications
                .Include(o => o.User)
                    .ThenInclude(u => u.Wallet)
                .Where(o => o.User.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }

        public Task UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public async Task AddOtpVerificationAsync(OtpVerification otpVerification, CancellationToken cancellationToken)
        {
            await _context.OtpVerifications.AddAsync(otpVerification, cancellationToken);
        }

        public async Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            await _context.Transactions.AddAsync(transaction, cancellationToken);
        }

        public async Task AddOwnerUpgradePaymentAsync(OwnerUpgradePayment ownerUpgradePayment, CancellationToken cancellationToken)
        {
            await _context.OwnerUpgradePayments.AddAsync(ownerUpgradePayment, cancellationToken);
        }

        public async Task<Transaction?> GetTransactionByCodeAsync(string transactionCode, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Include(t => t.OwnerUpgradePayment)
                    .ThenInclude(oup => oup!.OwnerProfileRequest)
                .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode, cancellationToken);
        }

        public async Task<OwnerProfileRequest?> GetLatestPendingOwnerProfileRequestAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.OwnerProfileRequests
                .Where(o => o.UserId == userId && o.Status == OwnerProfileRequestStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddOwnerProfileRequestAsync(OwnerProfileRequest request, CancellationToken cancellationToken)
        {
            await _context.OwnerProfileRequests.AddAsync(request, cancellationToken);
        }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.CommitAsync(cancellationToken);
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.RollbackAsync(cancellationToken);
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    System.Console.WriteLine($"[CONCURRENCY ERROR] Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                }
                throw;
            }
        }
    }
}
