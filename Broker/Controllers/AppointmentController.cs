using Broker.DTOs.Appointment;
using Broker.DTOs.Common;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetAppointment(int id)
        {
            var userId = User.GetUserId();
            var result = await _appointmentService.GetAppointmentByIdAsync(id, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<AppointmentDto>.ErrorResponse("قرار ملاقات یافت نشد"));
            }

            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(result));
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetAppointmentsByRequest(int requestId)
        {
            var userId = User.GetUserId();
            var result = await _appointmentService.GetAppointmentsByRequestIdAsync(requestId, userId);
            return Ok(ApiResponse<List<AppointmentDto>>.SuccessResponse(result));
        }

        [HttpGet("my-appointments")]
        public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetMyAppointments(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<List<AppointmentDto>>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _appointmentService.GetMyAppointmentsAsync(userId, fromDate, toDate);
            return Ok(ApiResponse<List<AppointmentDto>>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> CreateAppointment([FromBody] CreateAppointmentDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AppointmentDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<AppointmentDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _appointmentService.CreateAppointmentAsync(userId, createDto);

            if (result == null)
            {
                return BadRequest(ApiResponse<AppointmentDto>.ErrorResponse("ایجاد قرار ملاقات انجام نشد"));
            }

            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(result, "قرار ملاقات با موفقیت ایجاد شد"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AppointmentDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<AppointmentDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _appointmentService.UpdateAppointmentAsync(id, updateDto, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<AppointmentDto>.ErrorResponse("قرار ملاقات یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(result, "قرار ملاقات با موفقیت به‌روزرسانی شد"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAppointment(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _appointmentService.DeleteAppointmentAsync(id, userId);

            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("حذف قرار ملاقات انجام نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "قرار ملاقات با موفقیت حذف شد"));
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AppointmentDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<AppointmentDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, statusDto, userId);

            if (result == null)
            {
                return NotFound(ApiResponse<AppointmentDto>.ErrorResponse("قرار ملاقات یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(result, "وضعیت قرار ملاقات با موفقیت به‌روزرسانی شد"));
        }
    }
}


