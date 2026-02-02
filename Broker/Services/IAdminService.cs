using Broker.DTOs.Admin;

namespace Broker.Services
{
    public interface IAdminService
    {
        Task<AdminUserListDto> GetUsersAsync(string? role = null, bool? isActive = null, bool? isVerified = null, int pageNumber = 1, int pageSize = 10);
        Task<AdminUserDto?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserStatusAsync(int userId, UpdateUserStatusDto updateDto);
        Task<List<VerificationRequestDto>> GetPendingVerificationsAsync();
        Task<bool> ApproveVerificationAsync(int verificationId, int adminUserId, ApproveVerificationDto approveDto);
        Task<bool> RejectVerificationAsync(int verificationId, int adminUserId, RejectVerificationDto rejectDto);
        Task<StatisticsOverviewDto> GetStatisticsOverviewAsync();
        Task<List<DailyStatisticsDto>> GetDailyStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<RequestsReportDto> GetRequestsReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<UsersReportDto> GetUsersReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<AnnouncementDto> CreateAnnouncementAsync(int adminUserId, CreateAnnouncementDto createDto);
        Task<ActivityLogListDto> GetActivityLogsAsync(int? userId = null, string? action = null, int pageNumber = 1, int pageSize = 20);
    }
}

