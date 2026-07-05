using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class OwnerProfileRequest
    {
        public Guid RequestId { get; set; }
        public Guid UserId { get; set; }
        public string IdCardNumber { get; set; } = string.Empty;
        public string? RawDocumentsUrl { get; set; }
        public OwnerProfileRequestStatus Status { get; set; }
        public string? RejectReason { get; set; }
        public Guid? ApprovedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public User? ApprovedByAdmin { get; set; }
    }
}
