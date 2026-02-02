using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Rating
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string AgentCompanyName { get; set; } = string.Empty;
        public int RaterId { get; set; }
        public string RaterName { get; set; } = string.Empty;
        public int? RequestId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRatingDto
    {
        [Required]
        public int AgentId { get; set; }

        public int? RequestId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }

    public class UpdateRatingDto
    {
        [Range(1, 5)]
        public int? Score { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }

    public class RatingSummaryDto
    {
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new(); // Score -> Count
        public List<RatingDto> RecentRatings { get; set; } = new();
    }
}


