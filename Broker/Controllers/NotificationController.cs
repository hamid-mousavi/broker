using Broker.DTOs.Common;
using Broker.DTOs.Notification;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<NotificationListDto>>> GetNotifications([FromQuery] bool? unreadOnly = false)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<NotificationListDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(ApiResponse<NotificationListDto>.SuccessResponse(result));
        }

        [HttpGet("unread")]
        public async Task<ActionResult<ApiResponse<NotificationListDto>>> GetUnreadNotifications()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<NotificationListDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _notificationService.GetUserNotificationsAsync(userId, true);
            return Ok(ApiResponse<NotificationListDto>.SuccessResponse(result));
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult<ApiResponse<NotificationDto>>> MarkAsRead(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<NotificationDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _notificationService.MarkAsReadAsync(id, userId);
            if (result == null)
            {
                return NotFound(ApiResponse<NotificationDto>.ErrorResponse("اعلان یافت نشد"));
            }

            return Ok(ApiResponse<NotificationDto>.SuccessResponse(result, "اعلان به عنوان خوانده شده علامت‌گذاری شد"));
        }

        [HttpPut("read-all")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "همه اعلان‌ها به عنوان خوانده شده علامت‌گذاری شدند"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteNotification(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _notificationService.DeleteNotificationAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("اعلان یافت نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "اعلان با موفقیت حذف شد"));
        }

        [HttpPost("preferences")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePreferences([FromBody] NotificationPreferencesDto preferences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _notificationService.UpdateNotificationPreferencesAsync(userId, preferences);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "تنظیمات اعلان با موفقیت به‌روزرسانی شد"));
        }
    }
}

