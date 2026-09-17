using System;

namespace RealEstateSystem.Domain.Entity
{
    public class PropertyPayment
    {
        public Guid PropertyPaymentId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public Guid? WalletTransactionId { get; set; }

        public Transaction? Transaction { get; set; }
        public Property Property { get; set; } = null!;
        public WalletTransaction? WalletTransaction { get; set; }
    }
}
