using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Rating
{
    public class ReviewReplyDto
    {
        public int Id { get; set; }
        public int RatingId { get; set; }
        public int RepliedByUserId { get; set; }
        public string RepliedByName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateReviewReplyDto
    {
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
    }

    public class ReportReviewDto
    {
        [StringLength(500)]
        public string? Reason { get; set; }
    }
}

