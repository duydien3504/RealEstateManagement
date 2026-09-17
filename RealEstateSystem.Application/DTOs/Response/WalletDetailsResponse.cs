using System;
using System.Collections.Generic;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class WalletDetailsResponse
    {
        public Guid WalletId { get; set; }
        public decimal Balance { get; set; }
        public List<WalletTransactionDto> Transactions { get; set; } = new List<WalletTransactionDto>();
        public int TotalTransactions { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
