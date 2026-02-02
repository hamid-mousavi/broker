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
    [Route("api/cargo-owners")]
    [Authorize]
    public class CargoOwnersController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly ApplicationDbContext _context;

        public CargoOwnersController(IRequestService requestService, ApplicationDbContext context)
        {
            _requestService = requestService;
            _context = context;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<object>>> GetProfile()
        {
            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners
                .Include(co => co.User)
                .FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return NotFound(ApiResponse<object>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                cargoOwner.Id,
                cargoOwner.CompanyName,
                cargoOwner.NationalId,
                cargoOwner.EconomicCode,
                cargoOwner.Address,
                cargoOwner.City,
                cargoOwner.Province,
                User = new
                {
                    cargoOwner.User.FirstName,
                    cargoOwner.User.LastName,
                    cargoOwner.User.Email,
                    cargoOwner.User.PhoneNumber
                }
            }));
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] Broker.DTOs.CargoOwner.UpdateCargoOwnerDto dto)
        {
            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return NotFound(ApiResponse<object>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            if (!string.IsNullOrWhiteSpace(dto.CompanyName))
                cargoOwner.CompanyName = dto.CompanyName;
            if (!string.IsNullOrWhiteSpace(dto.NationalId))
                cargoOwner.NationalId = dto.NationalId;
            if (!string.IsNullOrWhiteSpace(dto.EconomicCode))
                cargoOwner.EconomicCode = dto.EconomicCode;
            if (!string.IsNullOrWhiteSpace(dto.Address))
                cargoOwner.Address = dto.Address;
            if (!string.IsNullOrWhiteSpace(dto.City))
                cargoOwner.City = dto.City;
            if (!string.IsNullOrWhiteSpace(dto.Province))
                cargoOwner.Province = dto.Province;

            cargoOwner.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(true, "پروفایل با موفقیت به‌روزرسانی شد"));
        }

        [HttpPost("requests")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> CreateRequest([FromBody] CreateRequestDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            var result = await _requestService.CreateRequestAsync(cargoOwner.Id, createDto);
            if (result == null)
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("ایجاد درخواست انجام نشد"));

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "درخواست با موفقیت ایجاد شد"));
        }

        [HttpGet("requests")]
        public async Task<ActionResult<ApiResponse<RequestListResponseDto>>> GetMyRequests(
            [FromQuery] int? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = User.GetUserId();
            Models.RequestStatus? requestStatus = status.HasValue ? (Models.RequestStatus?)status.Value : null;
            var result = await _requestService.GetMyRequestsAsync(userId, requestStatus, pageNumber, pageSize);
            return Ok(ApiResponse<RequestListResponseDto>.SuccessResponse(result));
        }

        [HttpGet("requests/{id}")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> GetRequest(int id)
        {
            var userId = User.GetUserId();
            var result = await _requestService.GetRequestByIdAsync(id, userId);
            if (result == null)
                return NotFound(ApiResponse<RequestDto>.ErrorResponse("درخواست یافت نشد"));
            return Ok(ApiResponse<RequestDto>.SuccessResponse(result));
        }

        [HttpPut("requests/{id}")]
        public async Task<ActionResult<ApiResponse<RequestDto>>> UpdateRequest(int id, [FromBody] UpdateRequestDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RequestDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            var result = await _requestService.UpdateRequestAsync(id, updateDto, userId);
            if (result == null)
                return NotFound(ApiResponse<RequestDto>.ErrorResponse("درخواست یافت نشد یا دسترسی ندارید"));

            return Ok(ApiResponse<RequestDto>.SuccessResponse(result, "درخواست با موفقیت به‌روزرسانی شد"));
        }

        [HttpDelete("requests/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelRequest(int id)
        {
            var userId = User.GetUserId();
            var result = await _requestService.DeleteRequestAsync(id, userId);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("حذف درخواست انجام نشد"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "درخواست با موفقیت لغو شد"));
        }

        [HttpGet("favorites")]
        public async Task<ActionResult<ApiResponse<object>>> GetFavorites()
        {
            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return NotFound(ApiResponse<object>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            var favorites = await _context.Favorites
                .Include(f => f.Agent)
                    .ThenInclude(a => a.User)
                .Where(f => f.CargoOwnerId == cargoOwner.Id)
                .Select(f => new
                {
                    f.Id,
                    Agent = new
                    {
                        f.Agent.Id,
                        f.Agent.CompanyName,
                        f.Agent.AverageRating,
                        f.Agent.CompletedRequests
                    }
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(favorites));
        }

        [HttpPost("favorites/{brokerId}")]
        public async Task<ActionResult<ApiResponse<bool>>> AddFavorite(int brokerId)
        {
            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return NotFound(ApiResponse<bool>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            var exists = await _context.Favorites
                .AnyAsync(f => f.CargoOwnerId == cargoOwner.Id && f.AgentId == brokerId);

            if (exists)
                return BadRequest(ApiResponse<bool>.ErrorResponse("این ترخیص‌کار قبلاً ذخیره شده است"));

            _context.Favorites.Add(new Models.Favorite
            {
                CargoOwnerId = cargoOwner.Id,
                AgentId = brokerId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<bool>.SuccessResponse(true, "ترخیص‌کار با موفقیت ذخیره شد"));
        }

        [HttpDelete("favorites/{brokerId}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFavorite(int brokerId)
        {
            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return NotFound(ApiResponse<bool>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.CargoOwnerId == cargoOwner.Id && f.AgentId == brokerId);

            if (favorite == null)
                return NotFound(ApiResponse<bool>.ErrorResponse("ترخیص‌کار در لیست ذخیره‌ها یافت نشد"));

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse<bool>.SuccessResponse(true, "ترخیص‌کار از لیست ذخیره‌ها حذف شد"));
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboard()
        {
            var userId = User.GetUserId();
            var cargoOwner = await _context.CargoOwners.FirstOrDefaultAsync(co => co.UserId == userId);

            if (cargoOwner == null)
                return NotFound(ApiResponse<object>.ErrorResponse("پروفایل صاحب کالا یافت نشد"));

            var totalRequests = await _context.Requests.CountAsync(r => r.CargoOwnerId == cargoOwner.Id);
            var pendingRequests = await _context.Requests.CountAsync(r => r.CargoOwnerId == cargoOwner.Id && r.Status == Models.RequestStatus.Pending);
            var completedRequests = await _context.Requests.CountAsync(r => r.CargoOwnerId == cargoOwner.Id && r.Status == Models.RequestStatus.Completed);
            var favoritesCount = await _context.Favorites.CountAsync(f => f.CargoOwnerId == cargoOwner.Id);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                TotalRequests = totalRequests,
                PendingRequests = pendingRequests,
                CompletedRequests = completedRequests,
                FavoritesCount = favoritesCount
            }));
        }
    }
}

