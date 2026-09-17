namespace RealEstateSystem.Application.DTOs.Response
{
    public class VnpayReturnResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionCode { get; set; } = string.Empty;
        public string? ResponseCode { get; set; }
    }
}
