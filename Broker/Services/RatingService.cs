using Broker.Data;
using Broker.DTOs.Rating;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class RatingService : IRatingService
    {
        private readonly ApplicationDbContext _context;

        public RatingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RatingDto?> GetRatingByIdAsync(int ratingId)
        {
            var rating = await _context.Ratings
                .Include(r => r.Agent)
                .Include(r => r.Rater)
                .FirstOrDefaultAsync(r => r.Id == ratingId);

            if (rating == null) return null;

            return MapToDto(rating);
        }

        public async Task<List<RatingDto>> GetRatingsByAgentIdAsync(int agentId, int pageNumber = 1, int pageSize = 10)
        {
            var ratings = await _context.Ratings
                .Include(r => r.Agent)
                .Include(r => r.Rater)
                .Where(r => r.AgentId == agentId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ratings.Select(MapToDto).ToList();
        }

        public async Task<RatingSummaryDto> GetRatingSummaryAsync(int agentId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.AgentId == agentId)
                .ToListAsync();

            if (!ratings.Any())
            {
                return new RatingSummaryDto
                {
                    AverageRating = 0,
                    TotalRatings = 0,
                    RatingDistribution = new Dictionary<int, int>(),
                    RecentRatings = new List<RatingDto>()
                };
            }

            var averageRating = ratings.Average(r => r.Score);
            var totalRatings = ratings.Count;

            var distribution = ratings
                .GroupBy(r => r.Score)
                .ToDictionary(g => g.Key, g => g.Count());

            // Fill missing scores with 0
            for (int i = 1; i <= 5; i++)
            {
                if (!distribution.ContainsKey(i))
                    distribution[i] = 0;
            }

            var recentRatings = await _context.Ratings
                .Include(r => r.Agent)
                .Include(r => r.Rater)
                .Where(r => r.AgentId == agentId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new RatingSummaryDto
            {
                AverageRating = (decimal)averageRating,
                TotalRatings = totalRatings,
                RatingDistribution = distribution,
                RecentRatings = recentRatings.Select(MapToDto).ToList()
            };
        }

        public async Task<RatingDto?> CreateRatingAsync(int raterId, CreateRatingDto createDto)
        {
            var agent = await _context.ClearanceAgents.FindAsync(createDto.AgentId);
            if (agent == null) return null;

            // Check if rating already exists for this request
            if (createDto.RequestId.HasValue)
            {
                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.RequestId == createDto.RequestId.Value && r.RaterId == raterId);

                if (existingRating != null) return null; // Already rated
            }

            var rating = new Models.Rating
            {
                AgentId = createDto.AgentId,
                RaterId = raterId,
                RequestId = createDto.RequestId,
                Score = createDto.Score,
                Comment = createDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            // Update agent rating statistics
            await UpdateAgentRatingStatsAsync(createDto.AgentId);

            return await GetRatingByIdAsync(rating.Id);
        }

        public async Task<RatingDto?> UpdateRatingAsync(int ratingId, UpdateRatingDto updateDto, int raterId)
        {
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.Id == ratingId && r.RaterId == raterId);

            if (rating == null) return null;

            if (updateDto.Score.HasValue)
                rating.Score = updateDto.Score.Value;

            if (updateDto.Comment != null)
                rating.Comment = updateDto.Comment;

            rating.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Update agent rating statistics
            await UpdateAgentRatingStatsAsync(rating.AgentId);

            return await GetRatingByIdAsync(ratingId);
        }

        public async Task<bool> DeleteRatingAsync(int ratingId, int raterId)
        {
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.Id == ratingId && r.RaterId == raterId);

            if (rating == null) return false;

            var agentId = rating.AgentId;
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            // Update agent rating statistics
            await UpdateAgentRatingStatsAsync(agentId);

            return true;
        }

        public async Task<RatingDto?> GetRatingByRequestIdAsync(int requestId, int raterId)
        {
            var rating = await _context.Ratings
                .Include(r => r.Agent)
                .Include(r => r.Rater)
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.RaterId == raterId);

            if (rating == null) return null;

            return MapToDto(rating);
        }

        private async Task UpdateAgentRatingStatsAsync(int agentId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.AgentId == agentId)
                .ToListAsync();

            if (!ratings.Any()) return;

            var agent = await _context.ClearanceAgents.FindAsync(agentId);
            if (agent == null) return;

            agent.AverageRating = (decimal)ratings.Average(r => r.Score);
            agent.TotalRatings = ratings.Count;
            agent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private RatingDto MapToDto(Models.Rating rating)
        {
            return new RatingDto
            {
                Id = rating.Id,
                AgentId = rating.AgentId,
                AgentCompanyName = rating.Agent.CompanyName,
                RaterId = rating.RaterId,
                RaterName = $"{rating.Rater.FirstName} {rating.Rater.LastName}",
                RequestId = rating.RequestId,
                Score = rating.Score,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt
            };
        }
    }
}


