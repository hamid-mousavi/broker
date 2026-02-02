using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class VerificationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

        [StringLength(500)]
        public string? AdminNotes { get; set; }

        public int? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("AgentId")]
        public ClearanceAgent Agent { get; set; } = null!;

        [ForeignKey("ReviewedByUserId")]
        public User? ReviewedByUser { get; set; }
    }

    public enum VerificationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}

