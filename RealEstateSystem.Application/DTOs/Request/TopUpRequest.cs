namespace RealEstateSystem.Application.DTOs.Request
{
    public class TopUpRequest
    {
        public decimal Amount { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
