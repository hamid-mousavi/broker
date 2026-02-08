using Broker.DTOs.Admin;
using Broker.DTOs.Common;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsAdmin()
        {
            var role = User.GetUserRole();
            return role == "Admin";
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<AdminUserListDto>>> GetUsers(
            [FromQuery] string? role = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? isVerified = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetUsersAsync(role, isActive, isVerified, pageNumber, pageSize);
            return Ok(ApiResponse<AdminUserListDto>.SuccessResponse(result));
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> GetUser(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetUserByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<AdminUserDto>.ErrorResponse("کاربر یافت نشد"));

            return Ok(ApiResponse<AdminUserDto>.SuccessResponse(result));
        }

        [HttpPut("users/{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDto updateDto)
        {
            if (!IsAdmin())
                return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _adminService.UpdateUserStatusAsync(id, updateDto);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("کاربر یافت نشد"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "وضعیت کاربر با موفقیت به‌روزرسانی شد"));
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> UpdateUser(int id, [FromBody] UpdateAdminUserDto updateDto)
        {
            if (!IsAdmin())
                return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AdminUserDto>.ErrorResponse("Invalid request data",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var (user, error) = await _adminService.UpdateUserAsync(id, updateDto);
            if (user == null)
            {
                if (error == "NOT_FOUND")
                    return NotFound(ApiResponse<AdminUserDto>.ErrorResponse("User not found"));

                return BadRequest(ApiResponse<AdminUserDto>.ErrorResponse(error ?? "Update failed"));
            }

            return Ok(ApiResponse<AdminUserDto>.SuccessResponse(user, "User updated successfully"));
        }

        [HttpPut("users/{id}/verify")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUserVerification(int id, [FromBody] UpdateUserVerificationDto updateDto)
        {
            if (!IsAdmin())
                return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _adminService.UpdateUserVerificationAsync(id, updateDto);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User verification updated"));
        }

        [HttpPut("users/{id}/password")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetUserPassword(int id, [FromBody] AdminResetPasswordDto updateDto)
        {
            if (!IsAdmin())
                return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _adminService.ResetUserPasswordAsync(id, updateDto);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Password updated successfully"));
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.DeleteUserAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
        }

        [HttpGet("verifications/pending")]
        public async Task<ActionResult<ApiResponse<List<VerificationRequestDto>>>> GetPendingVerifications()
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetPendingVerificationsAsync();
            return Ok(ApiResponse<List<VerificationRequestDto>>.SuccessResponse(result));
        }

        [HttpPost("verifications/{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveVerification(int id, [FromBody] ApproveVerificationDto approveDto)
        {
            if (!IsAdmin())
                return Forbid();

            var adminUserId = User.GetUserId();
            if (adminUserId == 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));

            var result = await _adminService.ApproveVerificationAsync(id, adminUserId, approveDto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("تایید انجام نشد. ممکن است درخواست در وضعیت مناسب نباشد"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "مدارک با موفقیت تایید شد"));
        }

        [HttpPost("verifications/{id}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> RejectVerification(int id, [FromBody] RejectVerificationDto rejectDto)
        {
            if (!IsAdmin())
                return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var adminUserId = User.GetUserId();
            if (adminUserId == 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));

            var result = await _adminService.RejectVerificationAsync(id, adminUserId, rejectDto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("رد انجام نشد. ممکن است درخواست در وضعیت مناسب نباشد"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "مدارک رد شد"));
        }

        [HttpGet("documents/pending")]
        public async Task<ActionResult<ApiResponse<List<AdminDocumentDto>>>> GetPendingDocuments()
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetPendingDocumentsAsync();
            return Ok(ApiResponse<List<AdminDocumentDto>>.SuccessResponse(result));
        }

        [HttpPost("documents/{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveDocument(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.ApproveDocumentAsync(id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("تایید انجام نشد"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "مدرک با موفقیت تایید شد"));
        }

        [HttpPost("documents/{id}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> RejectDocument(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.RejectDocumentAsync(id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("رد انجام نشد"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "مدرک رد شد"));
        }

        [HttpGet("statistics/overview")]
        public async Task<ActionResult<ApiResponse<StatisticsOverviewDto>>> GetStatisticsOverview()
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetStatisticsOverviewAsync();
            return Ok(ApiResponse<StatisticsOverviewDto>.SuccessResponse(result));
        }

        [HttpGet("statistics/daily")]
        public async Task<ActionResult<ApiResponse<List<DailyStatisticsDto>>>> GetDailyStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetDailyStatisticsAsync(startDate, endDate);
            return Ok(ApiResponse<List<DailyStatisticsDto>>.SuccessResponse(result));
        }

        [HttpGet("reports/requests")]
        public async Task<ActionResult<ApiResponse<RequestsReportDto>>> GetRequestsReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetRequestsReportAsync(startDate, endDate);
            return Ok(ApiResponse<RequestsReportDto>.SuccessResponse(result));
        }

        [HttpGet("reports/users")]
        public async Task<ActionResult<ApiResponse<UsersReportDto>>> GetUsersReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetUsersReportAsync(startDate, endDate);
            return Ok(ApiResponse<UsersReportDto>.SuccessResponse(result));
        }

        [HttpPost("announcements")]
        public async Task<ActionResult<ApiResponse<AnnouncementDto>>> CreateAnnouncement([FromBody] CreateAnnouncementDto createDto)
        {
            if (!IsAdmin())
                return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AnnouncementDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var adminUserId = User.GetUserId();
            if (adminUserId == 0)
                return Unauthorized(ApiResponse<AnnouncementDto>.ErrorResponse("کاربر احراز هویت نشده است"));

            try
            {
                var result = await _adminService.CreateAnnouncementAsync(adminUserId, createDto);
                return Ok(ApiResponse<AnnouncementDto>.SuccessResponse(result, "اطلاعیه با موفقیت ایجاد شد"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("activity-logs")]
        public async Task<ActionResult<ApiResponse<ActivityLogListDto>>> GetActivityLogs(
            [FromQuery] int? userId = null,
            [FromQuery] string? action = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!IsAdmin())
                return Forbid();

            var result = await _adminService.GetActivityLogsAsync(userId, action, pageNumber, pageSize);
            return Ok(ApiResponse<ActivityLogListDto>.SuccessResponse(result));
        }
    }
}

