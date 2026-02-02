using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; } // Info, Success, Warning, Error

        [StringLength(500)]
        public string? Link { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}

