using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; } // Info, Warning, Important

        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CreatedByUserId")]
        public User CreatedByUser { get; set; } = null!;
    }
}

