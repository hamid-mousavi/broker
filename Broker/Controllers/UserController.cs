using Broker.DTOs.Common;
using Broker.DTOs.User;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<UserProfileDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _userService.GetUserProfileAsync(userId);

            if (result == null)
            {
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("کاربر یافت نشد"));
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(result));
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserProfileDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<UserProfileDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _userService.UpdateUserProfileAsync(userId, updateDto);

            if (result == null)
            {
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("کاربر یافت نشد"));
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(result, "پروفایل با موفقیت به‌روزرسانی شد"));
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
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

            var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("رمز عبور فعلی اشتباه است"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "رمز عبور با موفقیت تغییر یافت"));
        }

        [HttpDelete("deactivate")]
        public async Task<ActionResult<ApiResponse<bool>>> DeactivateAccount()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _userService.DeactivateAccountAsync(userId);

            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("کاربر یافت نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "حساب کاربری با موفقیت غیرفعال شد"));
        }
    }
}


