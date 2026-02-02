using Broker.Data;
using Broker.DTOs.Common;
using Broker.DTOs.Request;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly ApplicationDbContext _context;

        public RequestController(IRequestService requestService, ApplicationDbContext context)
        {
            _requestService = requestService;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> GetRequest(int id)
        {
            var userId = User.GetUserId();
            var result = await _requestService.GetRequestByIdAsync(id, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<RequestDto>.ErrorResponse("درخواست یافت نشد"));
            }

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result));
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<RequestListResponseDto>>> SearchRequests([FromBody] RequestSearchDto searchDto)
        {
            var userId = User.GetUserId();
            var result = await _requestService.GetRequestsAsync(searchDto, userId);
            return Ok(ApiResponse<RequestListResponseDto>.SuccessResponse(result));
        }

        [HttpGet("my-requests")]
        public async Task<ActionResult<ApiResponse<RequestListResponseDto?>>> GetMyRequests(
            [FromQuery] int? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RequestListResponseDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            Models.RequestStatus? requestStatus = status.HasValue ? (Models.RequestStatus?)status.Value : null;
            var result = await _requestService.GetMyRequestsAsync(userId, requestStatus, pageNumber, pageSize);
            return Ok(ApiResponse<RequestListResponseDto>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RequestDto>>> CreateRequest([FromBody] CreateRequestDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RequestDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            // Get cargo owner ID from user
            var cargoOwnerEntity = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);
            
            if (cargoOwnerEntity == null)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));
            }

            var result = await _requestService.CreateRequestAsync(cargoOwnerEntity.Id, createDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("ایجاد درخواست انجام نشد"));
            }

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "درخواست با موفقیت ایجاد شد"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> UpdateRequest(int id, [FromBody] UpdateRequestDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RequestDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _requestService.UpdateRequestAsync(id, updateDto, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<RequestDto>.ErrorResponse("درخواست یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "درخواست با موفقیت به‌روزرسانی شد"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRequest(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _requestService.DeleteRequestAsync(id, userId);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("حذف درخواست انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "درخواست با موفقیت حذف شد"));
        }

        [HttpPost("{id}/assign-agent")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> AssignAgent(int id, [FromBody] AssignAgentDto assignDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RequestDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _requestService.AssignAgentAsync(id, assignDto, userId);

            if (result == null)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اختصاص ترخیص‌کار انجام نشد"));
            }

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "ترخیص‌کار با موفقیت اختصاص یافت"));
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> UpdateStatus(int id, [FromBody] UpdateRequestStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<RequestDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _requestService.UpdateRequestStatusAsync(id, statusDto, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<RequestDto>.ErrorResponse("درخواست یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "وضعیت درخواست با موفقیت به‌روزرسانی شد"));
        }

        [HttpPost("{id}/response")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> RespondToRequest(int id, [FromBody] AssignAgentDto assignDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            var result = await _requestService.AssignAgentAsync(id, assignDto, userId);

            if (result == null)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("پاسخ به درخواست انجام نشد"));
            }

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "پاسخ با موفقیت ثبت شد"));
        }

        [HttpGet("{id}/messages")]
        public async Task<ActionResult<ApiResponse<object>>> GetRequestMessages(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.GetUserId();
            var messageService = HttpContext.RequestServices.GetRequiredService<IMessageService>();
            var searchDto = new Broker.DTOs.Message.MessageSearchDto
            {
                RequestId = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await messageService.GetMessagesAsync(searchDto, userId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpPost("{id}/messages")]
        public async Task<ActionResult<ApiResponse<object>>> SendRequestMessage(int id, [FromBody] Broker.DTOs.Message.CreateMessageDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            createDto.RequestId = id;
            var messageService = HttpContext.RequestServices.GetRequiredService<IMessageService>();
            var result = await messageService.SendMessageAsync(userId, createDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("ارسال پیام انجام نشد"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(result, "پیام با موفقیت ارسال شد"));
        }

        [HttpPut("{id}/rate")]
        public async Task<ActionResult<ApiResponse<object>>> RateRequest(int id, [FromBody] Broker.DTOs.Rating.CreateRatingDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            var request = await _requestService.GetRequestByIdAsync(id, userId);
            if (request == null || !request.AgentId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("درخواست یافت نشد یا ترخیص‌کار اختصاص نیافته است"));
            }

            createDto.RequestId = id;
            createDto.AgentId = request.AgentId.Value;
            var ratingService = HttpContext.RequestServices.GetRequiredService<IRatingService>();
            var result = await ratingService.CreateRatingAsync(userId, createDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("ثبت امتیاز انجام نشد"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(result, "امتیاز با موفقیت ثبت شد"));
        }
    }
}

