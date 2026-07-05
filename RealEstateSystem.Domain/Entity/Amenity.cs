namespace RealEstateSystem.Domain.Entity
{
    public class Amenity
    {
        public Guid AmenityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    }
}
