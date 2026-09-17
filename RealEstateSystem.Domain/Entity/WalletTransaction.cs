using System;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class WalletTransaction
    {
        public Guid WalletTransactionId { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public WalletTransactionType TransactionType { get; set; }
        public WalletReferenceType? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public Wallet Wallet { get; set; } = null!;
        public OwnerUpgradePayment? OwnerUpgradePayment { get; set; }
        public PropertyPayment? PropertyPayment { get; set; }
    }
}
