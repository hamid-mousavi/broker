using Broker.DTOs.Common;
using Broker.DTOs.Message;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MessageDto>>> GetMessage(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<MessageDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _messageService.GetMessageByIdAsync(id, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<MessageDto>.ErrorResponse("پیام یافت نشد"));
            }

            return Ok(ApiResponse<MessageDto>.SuccessResponse(result));
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<MessageListResponseDto>>> SearchMessages([FromBody] MessageSearchDto searchDto)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<MessageListResponseDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _messageService.GetMessagesAsync(searchDto, userId);
            return Ok(ApiResponse<MessageListResponseDto>.SuccessResponse(result));
        }

        [HttpGet("conversation")]
        public async Task<ActionResult<ApiResponse<MessageListResponseDto>>> GetConversation(
            [FromQuery] int requestId,
            [FromQuery] int otherUserId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<MessageListResponseDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _messageService.GetConversationAsync(requestId, otherUserId, userId, pageNumber, pageSize);
            return Ok(ApiResponse<MessageListResponseDto>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage([FromBody] CreateMessageDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MessageDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<MessageDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _messageService.SendMessageAsync(userId, createDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<MessageDto>.ErrorResponse("ارسال پیام انجام نشد"));
            }

            return Ok(ApiResponse<MessageDto>.SuccessResponse(result, "پیام با موفقیت ارسال شد"));
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _messageService.MarkAsReadAsync(id, userId);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("علامت‌گذاری پیام انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "پیام به عنوان خوانده شده علامت‌گذاری شد"));
        }

        [HttpPut("conversation/read")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkConversationAsRead(
            [FromQuery] int requestId,
            [FromQuery] int otherUserId)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _messageService.MarkConversationAsReadAsync(requestId, userId, otherUserId);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("علامت‌گذاری مکالمه انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "مکالمه به عنوان خوانده شده علامت‌گذاری شد"));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<int>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var count = await _messageService.GetUnreadMessageCountAsync(userId);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
    }
}


