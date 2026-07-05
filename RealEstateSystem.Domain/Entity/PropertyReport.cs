using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class PropertyReport
    {
        public Guid ReportId { get; set; }
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public User User { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}
