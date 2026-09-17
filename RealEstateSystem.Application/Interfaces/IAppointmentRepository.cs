using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
        Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken);
        Task<List<Appointment>> GetAppointmentsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
