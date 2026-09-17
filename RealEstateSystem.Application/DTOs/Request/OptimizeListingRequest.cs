namespace RealEstateSystem.Application.DTOs.Request
{
    public class OptimizeListingRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Area { get; set; }
    }
}
