using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class User
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public StatusType Status { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Role Role { get; set; } = null!;

        public ICollection<OtpVerification> OtpVerifications { get; set; } = new List<OtpVerification>();
        public ICollection<OwnerProfileRequest> OwnerProfileRequests { get; set; } = new List<OwnerProfileRequest>();
        public ICollection<OwnerProfileRequest> ApprovedOwnerProfileRequests { get; set; } = new List<OwnerProfileRequest>();
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
        public ICollection<PropertyRating> PropertyRatings { get; set; } = new List<PropertyRating>();
        public ICollection<PropertyReport> PropertyReports { get; set; } = new List<PropertyReport>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
