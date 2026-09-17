using System;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetWalletByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddWalletAsync(Wallet wallet, CancellationToken cancellationToken);
        Task AddWalletTransactionAsync(WalletTransaction walletTransaction, CancellationToken cancellationToken);
        Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
        Task<Transaction?> GetTransactionByCodeAsync(string transactionCode, CancellationToken cancellationToken);
        Task<WalletDetailsResponse?> GetWalletDetailsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
