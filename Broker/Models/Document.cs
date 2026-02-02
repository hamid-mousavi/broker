using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MimeType { get; set; }

        public long FileSize { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}

