using Broker.DTOs.Auth;
using Broker.DTOs.Common;
using Broker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<TokenResponseDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست", 
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<TokenResponseDto>.ErrorResponse("ایمیل قبلاً ثبت شده است"));
            }

            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "ثبت‌نام با موفقیت انجام شد"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<TokenResponseDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(ApiResponse<TokenResponseDto>.ErrorResponse("ایمیل یا رمز عبور اشتباه است"));
            }

            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "ورود با موفقیت انجام شد"));
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authService.VerifyEmailAsync(email, token);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("تایید ایمیل انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "ایمیل با موفقیت تایید شد"));
        }

        [HttpPost("resend-verification-email")]
        public async Task<ActionResult<ApiResponse<bool>>> ResendVerification([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.ResendVerificationEmailAsync(dto.Email);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("ارسال مجدد ایمیل تایید انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "ایمیل تایید مجدداً ارسال شد"));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<TokenResponseDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _authService.RefreshTokenAsync(dto.Token);

            if (result == null)
            {
                return Unauthorized(ApiResponse<TokenResponseDto>.ErrorResponse("توکن معتبر نیست یا منقضی شده است"));
            }

            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "توکن با موفقیت به‌روزرسانی شد"));
        }

        [HttpPost("verify-email/{token}")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyEmailByToken(string token, [FromQuery] string email)
        {
            var result = await _authService.VerifyEmailAsync(email, token);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("تایید ایمیل انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "ایمیل با موفقیت تایید شد"));
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _authService.ForgotPasswordAsync(dto.Email);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("ارسال ایمیل بازیابی رمز انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "ایمیل بازیابی رمز ارسال شد"));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("بازیابی رمز انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "رمز عبور با موفقیت تغییر یافت"));
        }

        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] Broker.DTOs.User.ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = Broker.Helpers.ClaimsHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var userService = HttpContext.RequestServices.GetRequiredService<Broker.Services.IUserService>();
            var result = await userService.ChangePasswordAsync(userId, dto);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("رمز عبور فعلی اشتباه است"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "رمز عبور با موفقیت تغییر یافت"));
        }

        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<ApiResponse<Broker.DTOs.User.UserProfileDto>>> GetProfile()
        {
            var userId = Broker.Helpers.ClaimsHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<Broker.DTOs.User.UserProfileDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var userService = HttpContext.RequestServices.GetRequiredService<Broker.Services.IUserService>();
            var result = await userService.GetUserProfileAsync(userId);

            if (result == null)
            {
                return NotFound(ApiResponse<Broker.DTOs.User.UserProfileDto>.ErrorResponse("کاربر یافت نشد"));
            }

            return Ok(ApiResponse<Broker.DTOs.User.UserProfileDto>.SuccessResponse(result));
        }

        [HttpPut("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<ApiResponse<Broker.DTOs.User.UserProfileDto>>> UpdateProfile([FromBody] Broker.DTOs.User.UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Broker.DTOs.User.UserProfileDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = Broker.Helpers.ClaimsHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<Broker.DTOs.User.UserProfileDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var userService = HttpContext.RequestServices.GetRequiredService<Broker.Services.IUserService>();
            var result = await userService.UpdateUserProfileAsync(userId, dto);

            if (result == null)
            {
                return NotFound(ApiResponse<Broker.DTOs.User.UserProfileDto>.ErrorResponse("کاربر یافت نشد"));
            }

            return Ok(ApiResponse<Broker.DTOs.User.UserProfileDto>.SuccessResponse(result, "پروفایل با موفقیت به‌روزرسانی شد"));
        }

        [HttpPost("upload-profile-image")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<ApiResponse<string>>> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("فایل ارسال نشده است"));
            }

            var userId = Broker.Helpers.ClaimsHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            // TODO: Implement file upload logic
            var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine("wwwroot", "uploads", "profiles", fileName);

            // Create directory if not exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user profile image path
            var context = HttpContext.RequestServices.GetRequiredService<Broker.Data.ApplicationDbContext>();
            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                user.ProfileImagePath = $"/uploads/profiles/{fileName}";
                user.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            return Ok(ApiResponse<string>.SuccessResponse($"/uploads/profiles/{fileName}", "عکس پروفایل با موفقیت آپلود شد"));
        }
    }
}


