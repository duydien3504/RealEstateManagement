namespace RealEstateSystem.Application.DTOs.Response
{
    public class PropertyReportResponse
    {
        public Guid ReportId { get; set; }
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
