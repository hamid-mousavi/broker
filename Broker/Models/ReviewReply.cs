using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class ReviewReply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RatingId { get; set; }

        [Required]
        public int RepliedByUserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("RatingId")]
        public Rating Rating { get; set; } = null!;

        [ForeignKey("RepliedByUserId")]
        public User RepliedByUser { get; set; } = null!;
    }
}

