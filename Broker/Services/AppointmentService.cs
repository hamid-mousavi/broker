using Broker.Data;
using Broker.DTOs.Appointment;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId, int? userId = null)
        {
            var query = _context.Appointments
                .Include(a => a.Request)
                    .ThenInclude(r => r.CargoOwner)
                        .ThenInclude(co => co.User)
                .Include(a => a.Request)
                    .ThenInclude(r => r.Agent)
                .Include(a => a.CreatedByUser)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(a =>
                    a.CreatedByUserId == userId.Value ||
                    (a.Request.CargoOwner.UserId == userId.Value) ||
                    (a.Request.AgentId.HasValue && a.Request.Agent!.UserId == userId.Value));
            }

            var appointment = await query.FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null) return null;

            return MapToDto(appointment);
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByRequestIdAsync(int requestId, int? userId = null)
        {
            var query = _context.Appointments
                .Include(a => a.Request)
                    .ThenInclude(r => r.CargoOwner)
                        .ThenInclude(co => co.User)
                .Include(a => a.Request)
                    .ThenInclude(r => r.Agent)
                .Include(a => a.CreatedByUser)
                .Where(a => a.RequestId == requestId);

            if (userId.HasValue)
            {
                query = query.Where(a =>
                    a.CreatedByUserId == userId.Value ||
                    (a.Request.CargoOwner.UserId == userId.Value) ||
                    (a.Request.AgentId.HasValue && a.Request.Agent!.UserId == userId.Value));
            }

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapToDto).ToList();
        }

        public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Appointments
                .Include(a => a.Request)
                    .ThenInclude(r => r.CargoOwner)
                        .ThenInclude(co => co.User)
                .Include(a => a.Request)
                    .ThenInclude(r => r.Agent)
                .Include(a => a.CreatedByUser)
                .Where(a =>
                    a.CreatedByUserId == userId ||
                    (a.Request.CargoOwner.UserId == userId) ||
                    (a.Request.AgentId.HasValue && a.Request.Agent!.UserId == userId));

            if (fromDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= toDate.Value);

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapToDto).ToList();
        }

        public async Task<AppointmentDto?> CreateAppointmentAsync(int userId, CreateAppointmentDto createDto)
        {
            var request = await _context.Requests
                .Include(r => r.CargoOwner)
                .Include(r => r.Agent)
                .FirstOrDefaultAsync(r => r.Id == createDto.RequestId);

            if (request == null) return null;

            // Check if user has permission (must be cargo owner or assigned agent)
            var hasPermission = request.CargoOwner.UserId == userId ||
                               (request.AgentId.HasValue && request.Agent!.UserId == userId);

            if (!hasPermission) return null;

            var appointment = new Models.Appointment
            {
                RequestId = createDto.RequestId,
                CreatedByUserId = userId,
                AppointmentDate = createDto.AppointmentDate,
                Location = createDto.Location,
                Notes = createDto.Notes,
                Status = Models.AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return await GetAppointmentByIdAsync(appointment.Id);
        }

        public async Task<AppointmentDto?> UpdateAppointmentAsync(int appointmentId, UpdateAppointmentDto updateDto, int userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Request)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return null;

            // Check permissions
            var canUpdate = appointment.CreatedByUserId == userId ||
                           appointment.Request.CargoOwner.UserId == userId ||
                           (appointment.Request.AgentId.HasValue && appointment.Request.Agent!.UserId == userId);

            if (!canUpdate) return null;

            if (updateDto.AppointmentDate.HasValue)
                appointment.AppointmentDate = updateDto.AppointmentDate.Value;

            if (updateDto.Location != null)
                appointment.Location = updateDto.Location;

            if (updateDto.Notes != null)
                appointment.Notes = updateDto.Notes;

            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId, int userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Request)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return false;

            // Only creator can delete
            if (appointment.CreatedByUserId != userId) return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AppointmentDto?> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusDto statusDto, int userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Request)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return null;

            // Check permissions
            var canUpdate = appointment.CreatedByUserId == userId ||
                           appointment.Request.CargoOwner.UserId == userId ||
                           (appointment.Request.AgentId.HasValue && appointment.Request.Agent!.UserId == userId);

            if (!canUpdate) return null;

            appointment.Status = statusDto.Status;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetAppointmentByIdAsync(appointmentId);
        }

        private AppointmentDto MapToDto(Models.Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                RequestId = appointment.RequestId,
                RequestTitle = appointment.Request.Title,
                CreatedByUserId = appointment.CreatedByUserId,
                CreatedByName = $"{appointment.CreatedByUser.FirstName} {appointment.CreatedByUser.LastName}",
                AppointmentDate = appointment.AppointmentDate,
                Location = appointment.Location,
                Notes = appointment.Notes,
                Status = appointment.Status,
                StatusName = appointment.Status.ToString(),
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}


