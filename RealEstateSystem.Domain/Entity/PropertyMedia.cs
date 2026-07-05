using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class PropertyMedia
    {
        public Guid MediaId { get; set; }
        public Guid PropertyId { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public DateTime CreatedAt { get; set; }

        public Property Property { get; set; } = null!;
    }
}
