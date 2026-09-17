using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .Include(w => w.WalletTransactions)
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted, cancellationToken);
        }

        public async Task AddWalletAsync(Wallet wallet, CancellationToken cancellationToken)
        {
            await _context.Wallets.AddAsync(wallet, cancellationToken);
        }

        public async Task AddWalletTransactionAsync(WalletTransaction walletTransaction, CancellationToken cancellationToken)
        {
            await _context.WalletTransactions.AddAsync(walletTransaction, cancellationToken);
        }

        public async Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            await _context.Transactions.AddAsync(transaction, cancellationToken);
        }

        public async Task<Transaction?> GetTransactionByCodeAsync(string transactionCode, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode, cancellationToken);
        }

        public async Task<WalletDetailsResponse?> GetWalletDetailsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var walletInfo = await _context.Wallets
                .Select(w => new { w.WalletId, w.UserId })
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

            if (walletInfo == null)
            {
                return null;
            }

            var walletId = walletInfo.WalletId;

            var totalTopUp = await _context.WalletTransactions
                .Where(wt => wt.WalletId == walletId && (wt.TransactionType == WalletTransactionType.TopUp || wt.TransactionType == WalletTransactionType.Refund))
                .SumAsync(wt => (decimal?)wt.Amount, cancellationToken) ?? 0;

            var totalPayment = await _context.WalletTransactions
                .Where(wt => wt.WalletId == walletId && wt.TransactionType == WalletTransactionType.Payment)
                .SumAsync(wt => (decimal?)wt.Amount, cancellationToken) ?? 0;

            var balance = totalTopUp - totalPayment;

            var transactions = await _context.WalletTransactions
                .Where(wt => wt.WalletId == walletId)
                .OrderByDescending(wt => wt.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(wt => new WalletTransactionDto
                {
                    WalletTransactionId = wt.WalletTransactionId,
                    Amount = wt.Amount,
                    TransactionType = wt.TransactionType.ToString(),
                    ReferenceType = wt.ReferenceType != null ? wt.ReferenceType.ToString() : null,
                    ReferenceId = wt.ReferenceId,
                    Description = wt.Description,
                    CreatedAt = wt.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var totalTransactions = await _context.WalletTransactions
                .CountAsync(wt => wt.WalletId == walletId, cancellationToken);

            return new WalletDetailsResponse
            {
                WalletId = walletId,
                Balance = balance,
                Transactions = transactions,
                TotalTransactions = totalTransactions,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
