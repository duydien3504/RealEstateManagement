using RealEstateSystem.Domain.Common;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class Appointment : AuditableEntity
    {
        public Guid AppointmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? RejectReason { get; set; }

        public User User { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}
