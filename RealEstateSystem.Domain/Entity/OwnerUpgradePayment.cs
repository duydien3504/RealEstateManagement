using System;

namespace RealEstateSystem.Domain.Entity
{
    public class OwnerUpgradePayment
    {
        public Guid OwnerUpgradePaymentId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid OwnerProfileRequestId { get; set; }
        public Guid? WalletTransactionId { get; set; }

        public Transaction? Transaction { get; set; }
        public OwnerProfileRequest OwnerProfileRequest { get; set; } = null!;
        public WalletTransaction? WalletTransaction { get; set; }
    }
}
