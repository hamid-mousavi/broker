using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        public int RaterId { get; set; }

        public int? RequestId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("AgentId")]
        public ClearanceAgent Agent { get; set; } = null!;

        [ForeignKey("RaterId")]
        public User Rater { get; set; } = null!;

        [ForeignKey("RequestId")]
        public Request? Request { get; set; }

        public ICollection<ReviewReply> Replies { get; set; } = new List<ReviewReply>();
    }
}


