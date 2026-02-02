using Broker.DTOs.Appointment;

namespace Broker.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId, int? userId = null);
        Task<List<AppointmentDto>> GetAppointmentsByRequestIdAsync(int requestId, int? userId = null);
        Task<List<AppointmentDto>> GetMyAppointmentsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<AppointmentDto?> CreateAppointmentAsync(int userId, CreateAppointmentDto createDto);
        Task<AppointmentDto?> UpdateAppointmentAsync(int appointmentId, UpdateAppointmentDto updateDto, int userId);
        Task<bool> DeleteAppointmentAsync(int appointmentId, int userId);
        Task<AppointmentDto?> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusDto statusDto, int userId);
    }
}

