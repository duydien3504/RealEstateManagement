namespace RealEstateSystem.Domain.Entity
{
    public class PropertyRating
    {
        public Guid RatingId { get; set; }
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}
