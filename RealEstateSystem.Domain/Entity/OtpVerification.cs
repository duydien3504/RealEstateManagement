using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class OtpVerification
    {
        public Guid OtpId { get; set; }
        public Guid UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public OtpPurpose Purpose { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
