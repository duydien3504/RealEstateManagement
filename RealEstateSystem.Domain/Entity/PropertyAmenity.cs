namespace RealEstateSystem.Domain.Entity
{
    public class PropertyAmenity
    {
        public Guid PropertyAmenityId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid AmenityId { get; set; }

        public Property Property { get; set; } = null!;
        public Amenity Amenity { get; set; } = null!;
    }
}
