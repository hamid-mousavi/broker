using Broker.DTOs.Rating;

namespace Broker.Services
{
    public interface IRatingService
    {
        Task<RatingDto?> GetRatingByIdAsync(int ratingId);
        Task<List<RatingDto>> GetRatingsByAgentIdAsync(int agentId, int pageNumber = 1, int pageSize = 10);
        Task<RatingSummaryDto> GetRatingSummaryAsync(int agentId);
        Task<RatingDto?> CreateRatingAsync(int raterId, CreateRatingDto createDto);
        Task<RatingDto?> UpdateRatingAsync(int ratingId, UpdateRatingDto updateDto, int raterId);
        Task<bool> DeleteRatingAsync(int ratingId, int raterId);
        Task<RatingDto?> GetRatingByRequestIdAsync(int requestId, int raterId);
    }
}


