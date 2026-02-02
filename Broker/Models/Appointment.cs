using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestId { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("RequestId")]
        public Request Request { get; set; } = null!;

        [ForeignKey("CreatedByUserId")]
        public User CreatedByUser { get; set; } = null!;
    }

    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4
    }
}


