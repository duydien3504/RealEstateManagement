using System;
using System.Collections.Generic;

namespace RealEstateSystem.Domain.Entity
{
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
        public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
