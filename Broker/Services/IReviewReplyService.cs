using Broker.DTOs.Rating;

namespace Broker.Services
{
    public interface IReviewReplyService
    {
        Task<List<ReviewReplyDto>> GetRepliesByRatingIdAsync(int ratingId);
        Task<ReviewReplyDto?> CreateReplyAsync(int ratingId, int userId, CreateReviewReplyDto createDto);
        Task<bool> ReportReviewAsync(int ratingId, int userId, ReportReviewDto reportDto);
    }
}

