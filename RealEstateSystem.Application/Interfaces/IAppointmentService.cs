using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAppointmentAsync(Guid userId, CreateAppointmentRequest request, CancellationToken cancellationToken);
        Task<bool> CancelAppointmentAsync(Guid userId, Guid appointmentId, CancellationToken cancellationToken);
        Task<List<AppointmentResponseDto>> GetMyAppointmentsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
