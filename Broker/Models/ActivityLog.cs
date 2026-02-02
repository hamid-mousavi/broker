using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? EntityType { get; set; }

        public int? EntityId { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

