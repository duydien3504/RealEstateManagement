using System;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class WalletTransactionDto
    {
        public Guid WalletTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
