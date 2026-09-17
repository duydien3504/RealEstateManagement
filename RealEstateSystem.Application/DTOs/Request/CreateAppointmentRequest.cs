using System;

namespace RealEstateSystem.Application.DTOs.Request
{
    public class CreateAppointmentRequest
    {
        public Guid PropertyId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; } = string.Empty;
    }
}
