using Broker.DTOs.Common;
using Broker.DTOs.Rating;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IReviewReplyService _reviewReplyService;

        public RatingController(IRatingService ratingService, IReviewReplyService reviewReplyService)
        {
            _ratingService = ratingService;
            _reviewReplyService = reviewReplyService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RatingDto>>> GetRating(int id)
        {
            var result = await _ratingService.GetRatingByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<RatingDto>.ErrorResponse("امتیاز یافت نشد"));
            }

            return Ok(ApiResponse<RatingDto>.SuccessResponse(result));
        }

        [HttpGet("broker/{brokerId}")]
        public async Task<ActionResult<ApiResponse<List<RatingDto>>>> GetReviewsByBroker(
            int brokerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _ratingService.GetRatingsByAgentIdAsync(brokerId, pageNumber, pageSize);
            return Ok(ApiResponse<List<RatingDto>>.SuccessResponse(result));
        }

        [HttpGet("agent/{agentId}/summary")]
        public async Task<ActionResult<ApiResponse<RatingSummaryDto>>> GetRatingSummary(int agentId)
        {
            var result = await _ratingService.GetRatingSummaryAsync(agentId);
            return Ok(ApiResponse<RatingSummaryDto>.SuccessResponse(result));
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<ApiResponse<RatingDto>>> GetRatingByRequest(int requestId)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RatingDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _ratingService.GetRatingByRequestIdAsync(requestId, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<RatingDto>.ErrorResponse("امتیاز یافت نشد"));
            }

            return Ok(ApiResponse<RatingDto>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RatingDto>>> CreateRating([FromBody] CreateRatingDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RatingDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RatingDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _ratingService.CreateRatingAsync(userId, createDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<RatingDto>.ErrorResponse("ایجاد امتیاز انجام نشد. ممکن است قبلاً امتیاز داده باشید"));
            }

            return Ok(ApiResponse<RatingDto>.SuccessResponse(result, "امتیاز با موفقیت ثبت شد"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RatingDto>>> UpdateRating(int id, [FromBody] UpdateRatingDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RatingDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RatingDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _ratingService.UpdateRatingAsync(id, updateDto, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<RatingDto>.ErrorResponse("امتیاز یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<RatingDto>.SuccessResponse(result, "امتیاز با موفقیت به‌روزرسانی شد"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRating(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _ratingService.DeleteRatingAsync(id, userId);

            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("امتیاز یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "امتیاز با موفقیت حذف شد"));
        }

        [HttpGet("{id}/replies")]
        public async Task<ActionResult<ApiResponse<List<ReviewReplyDto>>>> GetReviewReplies(int id)
        {
            var result = await _reviewReplyService.GetRepliesByRatingIdAsync(id);
            return Ok(ApiResponse<List<ReviewReplyDto>>.SuccessResponse(result));
        }

        [HttpPost("{id}/replies")]
        public async Task<ActionResult<ApiResponse<ReviewReplyDto>>> AddReviewReply(int id, [FromBody] CreateReviewReplyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ReviewReplyDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<ReviewReplyDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _reviewReplyService.CreateReplyAsync(id, userId, dto);

            if (result == null)
            {
                return BadRequest(ApiResponse<ReviewReplyDto>.ErrorResponse("امکان ثبت پاسخ وجود ندارد. فقط ترخیص‌کار می‌تواند پاسخ دهد"));
            }

            return Ok(ApiResponse<ReviewReplyDto>.SuccessResponse(result, "پاسخ با موفقیت ثبت شد"));
        }

        [HttpPost("{id}/report")]
        public async Task<ActionResult<ApiResponse<bool>>> ReportReview(int id, [FromBody] ReportReviewDto dto)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _reviewReplyService.ReportReviewAsync(id, userId, dto);

            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("نظر یافت نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "گزارش با موفقیت ثبت شد"));
        }
    }
}


