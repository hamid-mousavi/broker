using Broker.DTOs.Agent;
using Broker.DTOs.Common;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/brokers")]
    public class BrokersController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly Broker.Data.ApplicationDbContext _context;

        public BrokersController(IAgentService agentService, Broker.Data.ApplicationDbContext context)
        {
            _agentService = agentService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<AgentListResponseDto>>> GetBrokers(
            [FromQuery] string? city = null,
            [FromQuery] string? province = null,
            [FromQuery] string? specialization = null,
            [FromQuery] int? minYearsOfExperience = null,
            [FromQuery] decimal? minRating = null,
            [FromQuery] bool? isVerified = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var searchDto = new AgentSearchDto
            {
                City = city,
                Province = province,
                Specialization = specialization,
                MinYearsOfExperience = minYearsOfExperience,
                MinRating = minRating,
                IsVerified = isVerified,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _agentService.SearchAgentsAsync(searchDto);
            return Ok(ApiResponse<AgentListResponseDto>.SuccessResponse(result));
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<AgentListResponseDto>>> SearchBrokers([FromBody] AgentSearchDto searchDto)
        {
            var result = await _agentService.SearchAgentsAsync(searchDto);
            return Ok(ApiResponse<AgentListResponseDto>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AgentProfileDto>>> GetBroker(int id)
        {
            var result = await _agentService.GetAgentProfileAsync(id);
            if (result == null)
                return NotFound(ApiResponse<AgentProfileDto>.ErrorResponse("ترخیص‌کار یافت نشد"));
            return Ok(ApiResponse<AgentProfileDto>.SuccessResponse(result));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AgentProfileDto>>> GetMyProfile()
        {
            var userId = User.GetUserId();
            if (userId == 0)
                return Unauthorized(ApiResponse<AgentProfileDto>.ErrorResponse("User not authenticated"));

            var result = await _agentService.GetAgentProfileByUserIdAsync(userId);
            if (result == null)
                return NotFound(ApiResponse<AgentProfileDto>.ErrorResponse("Broker profile not found"));

            return Ok(ApiResponse<AgentProfileDto>.SuccessResponse(result));
        }

        [HttpGet("{id}/services")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetBrokerServices(int id)
        {
            // TODO: Implement service list
            return Ok(ApiResponse<object>.SuccessResponse(new { }));
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<ApiResponse<object>>> GetBrokerReviews(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var ratingService = HttpContext.RequestServices.GetRequiredService<IRatingService>();
            var reviews = await ratingService.GetRatingsByAgentIdAsync(id, pageNumber, pageSize);
            return Ok(ApiResponse<object>.SuccessResponse(reviews));
        }

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<ApiResponse<object>>> GetBrokerStats(int id)
        {
            var ratingService = HttpContext.RequestServices.GetRequiredService<IRatingService>();
            var summary = await ratingService.GetRatingSummaryAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(summary));
        }

        [HttpGet("{id}/documents")]
        public async Task<ActionResult<ApiResponse<List<Broker.DTOs.Document.PublicDocumentDto>>>> GetBrokerDocuments(int id)
        {
            var agent = await _context.ClearanceAgents.FirstOrDefaultAsync(a => a.Id == id);
            if (agent == null)
                return NotFound(ApiResponse<List<Broker.DTOs.Document.PublicDocumentDto>>.ErrorResponse("ترخیص‌کار یافت نشد"));

            var docs = await _context.Documents
                .Where(d => d.UserId == agent.UserId && d.Status == Broker.Models.DocumentVerificationStatus.Approved)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new Broker.DTOs.Document.PublicDocumentDto
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType,
                    FilePath = d.FilePath,
                    Description = d.Description,
                    Status = d.Status.ToString(),
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<Broker.DTOs.Document.PublicDocumentDto>>.SuccessResponse(docs));
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AgentProfileDto>>> RegisterBroker([FromBody] CreateAgentProfileDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AgentProfileDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
                return Unauthorized(ApiResponse<AgentProfileDto>.ErrorResponse("کاربر احراز هویت نشده است"));

            var result = await _agentService.CreateAgentProfileAsync(userId, createDto);
            if (result == null)
                return BadRequest(ApiResponse<AgentProfileDto>.ErrorResponse("پروفایل قبلاً ایجاد شده است"));

            return Ok(ApiResponse<AgentProfileDto>.SuccessResponse(result, "پروفایل ترخیص‌کار با موفقیت ایجاد شد"));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AgentProfileDto>>> UpdateProfile([FromBody] UpdateAgentProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AgentProfileDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
                return Unauthorized(ApiResponse<AgentProfileDto>.ErrorResponse("کاربر احراز هویت نشده است"));

            var agent = await _agentService.GetAgentProfileByUserIdAsync(userId);
            if (agent == null)
                return NotFound(ApiResponse<AgentProfileDto>.ErrorResponse("پروفایل ترخیص‌کار یافت نشد"));

            var result = await _agentService.UpdateAgentProfileAsync(agent.Id, updateDto);
            if (result == null)
                return NotFound(ApiResponse<AgentProfileDto>.ErrorResponse("ترخیص‌کار یافت نشد"));

            return Ok(ApiResponse<AgentProfileDto>.SuccessResponse(result, "پروفایل با موفقیت به‌روزرسانی شد"));
        }

        [HttpPost("verification-request")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RequestVerification()
        {
            // TODO: Implement verification request
            return Ok(ApiResponse<bool>.SuccessResponse(true, "درخواست تایید با موفقیت ارسال شد"));
        }

        [HttpGet("dashboard/summary")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboardSummary()
        {
            // TODO: Implement dashboard summary
            return Ok(ApiResponse<object>.SuccessResponse(new { }));
        }

        [HttpGet("dashboard/requests")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboardRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.GetUserId();
            var requestService = HttpContext.RequestServices.GetRequiredService<IRequestService>();
            var requests = await requestService.GetMyRequestsAsync(userId, null, pageNumber, pageSize);
            return Ok(ApiResponse<object>.SuccessResponse(requests));
        }

        [HttpGet("dashboard/appointments")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboardAppointments()
        {
            var userId = User.GetUserId();
            var appointmentService = HttpContext.RequestServices.GetRequiredService<IAppointmentService>();
            var appointments = await appointmentService.GetMyAppointmentsAsync(userId);
            return Ok(ApiResponse<object>.SuccessResponse(appointments));
        }

        [HttpGet("dashboard/analytics")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboardAnalytics()
        {
            // TODO: Implement analytics
            return Ok(ApiResponse<object>.SuccessResponse(new { }));
        }
    }
}

