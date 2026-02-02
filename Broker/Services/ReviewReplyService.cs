using Broker.Data;
using Broker.DTOs.Rating;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class ReviewReplyService : IReviewReplyService
    {
        private readonly ApplicationDbContext _context;

        public ReviewReplyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewReplyDto>> GetRepliesByRatingIdAsync(int ratingId)
        {
            var replies = await _context.ReviewReplies
                .Include(r => r.RepliedByUser)
                .Where(r => r.RatingId == ratingId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            return replies.Select(r => new ReviewReplyDto
            {
                Id = r.Id,
                RatingId = r.RatingId,
                RepliedByUserId = r.RepliedByUserId,
                RepliedByName = $"{r.RepliedByUser.FirstName} {r.RepliedByUser.LastName}",
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }

        public async Task<ReviewReplyDto?> CreateReplyAsync(int ratingId, int userId, CreateReviewReplyDto createDto)
        {
            var rating = await _context.Ratings
                .Include(r => r.Agent)
                .FirstOrDefaultAsync(r => r.Id == ratingId);

            if (rating == null) return null;

            // Check if user is the agent who received the rating
            var agent = await _context.ClearanceAgents
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == rating.AgentId);

            if (agent == null) return null; // Only the agent can reply

            var reply = new Models.ReviewReply
            {
                RatingId = ratingId,
                RepliedByUserId = userId,
                Content = createDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReviewReplies.Add(reply);
            await _context.SaveChangesAsync();

            return await GetRepliesByRatingIdAsync(ratingId)
                .ContinueWith(t => t.Result.FirstOrDefault(r => r.Id == reply.Id));
        }

        public async Task<bool> ReportReviewAsync(int ratingId, int userId, ReportReviewDto reportDto)
        {
            var rating = await _context.Ratings.FindAsync(ratingId);
            if (rating == null) return false;

            // Create activity log for the report
            var activityLog = new Models.ActivityLog
            {
                UserId = userId,
                Action = "ReportReview",
                Description = $"User reported review {ratingId}. Reason: {reportDto.Reason}",
                EntityType = "Rating",
                EntityId = ratingId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

