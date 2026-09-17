using System;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid? WalletId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string? OrderInfo { get; set; }
        public string? ResponseCode { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public Wallet? Wallet { get; set; }
        public OwnerUpgradePayment? OwnerUpgradePayment { get; set; }
        public PropertyPayment? PropertyPayment { get; set; }
    }
}
