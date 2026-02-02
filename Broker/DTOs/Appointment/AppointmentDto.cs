using System.ComponentModel.DataAnnotations;
using Broker.Models;

namespace Broker.DTOs.Appointment
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string RequestTitle { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public AppointmentStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAppointmentDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public DateTime? AppointmentDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentStatusDto
    {
        [Required]
        public AppointmentStatus Status { get; set; }
    }
}

