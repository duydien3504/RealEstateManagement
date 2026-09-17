namespace RealEstateSystem.Application.DTOs.Response
{
    public class TransactionReportDto
    {
        public Guid TransactionId { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }
}
