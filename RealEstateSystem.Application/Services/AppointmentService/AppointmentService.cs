using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPropertyRepository _propertyRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPropertyRepository propertyRepository)
        {
            _appointmentRepository = appointmentRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(Guid userId, CreateAppointmentRequest request, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null || property.IsDeleted)
            {
                throw new NotFoundException("Không tìm thấy tin đăng bất động sản.");
            }

            if (!TimeSpan.TryParseExact(request.AppointmentTime, "g", CultureInfo.InvariantCulture, out var parsedTime) &&
                !TimeSpan.TryParse(request.AppointmentTime, out parsedTime))
            {
                throw new BadRequestException("Định dạng thời gian hẹn không hợp lệ (ví dụ hợp lệ: 14:30).");
            }

            var appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                UserId = userId,
                PropertyId = request.PropertyId,
                AppointmentDate = request.AppointmentDate.Date,
                AppointmentTime = parsedTime,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            var address = $"{property.AddressDetail}, {property.Ward?.Name}, {property.Ward?.Province?.Name}";

            return new AppointmentResponseDto
            {
                AppointmentId = appointment.AppointmentId,
                PropertyId = appointment.PropertyId,
                PropertyTitle = property.Title,
                PropertyAddress = address,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime.ToString(@"hh\:mm"),
                Status = appointment.Status.ToString(),
                RejectReason = appointment.RejectReason,
                CreatedAt = appointment.CreatedAt
            };
        }

        public async Task<bool> CancelAppointmentAsync(Guid userId, Guid appointmentId, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.UserId != userId)
            {
                return false;
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return true;
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<AppointmentResponseDto>> GetMyAppointmentsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByUserIdAsync(userId, cancellationToken);
            return appointments.Select(MapToResponseDto).ToList();
        }

        private AppointmentResponseDto MapToResponseDto(Appointment appointment)
        {
            var property = appointment.Property;
            var address = property != null 
                ? $"{property.AddressDetail}, {property.Ward?.Name}, {property.Ward?.Province?.Name}" 
                : string.Empty;

            return new AppointmentResponseDto
            {
                AppointmentId = appointment.AppointmentId,
                PropertyId = appointment.PropertyId,
                PropertyTitle = property?.Title ?? string.Empty,
                PropertyAddress = address,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime.ToString(@"hh\:mm"),
                Status = appointment.Status.ToString(),
                RejectReason = appointment.RejectReason,
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}
