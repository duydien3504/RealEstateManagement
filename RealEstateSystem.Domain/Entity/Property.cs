using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class Property
    {
        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid WardId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string? Dimensions { get; set; }
        public string? AddressDetail { get; set; }
        public int NumBedrooms { get; set; }
        public PropertyStatus PropertyStatusValue { get; set; }
        public DisplayStatus DisplayStatusValue { get; set; }
        public bool IsPremium { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User Owner { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public Ward Ward { get; set; } = null!;

        public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
        public ICollection<PropertyMedia> PropertyMedias { get; set; } = new List<PropertyMedia>();
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
        public ICollection<PropertyRating> PropertyRatings { get; set; } = new List<PropertyRating>();
        public ICollection<PropertyReport> PropertyReports { get; set; } = new List<PropertyReport>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
