namespace RealEstateSystem.Application.DTOs.Response
{
    public class TopUpResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? PaymentUrl { get; set; }
    }
}
