namespace RealEstateSystem.Application.DTOs.Request
{
    public class CreatePropertyReportRequest
    {
        public Guid PropertyId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
