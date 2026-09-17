namespace RealEstateSystem.Application.DTOs.Response
{
    public class PropertyReportDto
    {
        public Guid PropertyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string DisplayStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
