namespace RealEstateSystem.Domain.Entity
{
    public class UserFavorite
    {
        public Guid FavoriteId { get; set; }
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}
