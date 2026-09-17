using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            await _context.Appointments.AddAsync(appointment, cancellationToken);
        }

        public async Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken)
        {
            return await _context.Appointments
                .Include(a => a.Property)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId, cancellationToken);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Property)
                    .ThenInclude(p => p.Ward)
                        .ThenInclude(w => w.Province)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            var entry = _context.Entry(appointment);
            if (entry.State == EntityState.Detached)
            {
                _context.Appointments.Update(appointment);
            }
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
