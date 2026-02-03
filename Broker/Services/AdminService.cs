using Broker.Data;
using Broker.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminUserListDto> GetUsersAsync(string? role = null, bool? isActive = null, bool? isVerified = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                if (Enum.TryParse<Models.UserRole>(role, out var roleEnum))
                {
                    query = query.Where(u => u.Role == roleEnum);
                }
            }

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (isVerified.HasValue)
                query = query.Where(u => u.IsVerified == isVerified.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new AdminUserListDto
            {
                Users = users.Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsVerified = u.IsVerified,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new AdminUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, UpdateUserStatusDto updateDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = updateDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(AdminUserDto? User, string? Error)> UpdateUserAsync(int userId, UpdateAdminUserDto updateDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (null, "NOT_FOUND");

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == updateDto.Email && u.Id != userId);
            if (emailExists)
            {
                return (null, "ایمیل وارد شده قبلا ثبت شده است");
            }

            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (new AdminUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }, null);
        }

        public async Task<bool> UpdateUserVerificationAsync(int userId, UpdateUserVerificationDto updateDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsVerified = updateDto.IsVerified;
            user.UpdatedAt = DateTime.UtcNow;

            if (user.Role == Models.UserRole.ClearanceAgent)
            {
                var agent = await _context.ClearanceAgents.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (agent != null)
                {
                    agent.IsVerified = updateDto.IsVerified;
                    agent.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetUserPasswordAsync(int userId, AdminResetPasswordDto updateDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Soft delete to avoid FK issues
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VerificationRequestDto>> GetPendingVerificationsAsync()
        {
            var verifications = await _context.VerificationRequests
                .Include(v => v.Agent)
                    .ThenInclude(a => a.User)
                .Include(v => v.ReviewedByUser)
                .Where(v => v.Status == Models.VerificationStatus.Pending)
                .OrderBy(v => v.CreatedAt)
                .ToListAsync();

            return verifications.Select(v => new VerificationRequestDto
            {
                Id = v.Id,
                AgentId = v.AgentId,
                AgentCompanyName = v.Agent.CompanyName,
                AgentEmail = v.Agent.User.Email,
                Notes = v.Notes,
                Status = v.Status.ToString(),
                AdminNotes = v.AdminNotes,
                ReviewedByUserId = v.ReviewedByUserId,
                ReviewedByName = v.ReviewedByUser != null ? $"{v.ReviewedByUser.FirstName} {v.ReviewedByUser.LastName}" : null,
                ReviewedAt = v.ReviewedAt,
                CreatedAt = v.CreatedAt
            }).ToList();
        }

        public async Task<bool> ApproveVerificationAsync(int verificationId, int adminUserId, ApproveVerificationDto approveDto)
        {
            var verification = await _context.VerificationRequests
                .Include(v => v.Agent)
                .FirstOrDefaultAsync(v => v.Id == verificationId);

            if (verification == null || verification.Status != Models.VerificationStatus.Pending)
                return false;

            verification.Status = Models.VerificationStatus.Approved;
            verification.ReviewedByUserId = adminUserId;
            verification.ReviewedAt = DateTime.UtcNow;
            verification.AdminNotes = approveDto.AdminNotes;

            // Mark agent as verified
            verification.Agent.IsVerified = true;
            verification.Agent.UpdatedAt = DateTime.UtcNow;

            // Mark user as verified
            var user = await _context.Users.FindAsync(verification.Agent.UserId);
            if (user != null)
            {
                user.IsVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectVerificationAsync(int verificationId, int adminUserId, RejectVerificationDto rejectDto)
        {
            var verification = await _context.VerificationRequests
                .Include(v => v.Agent)
                .FirstOrDefaultAsync(v => v.Id == verificationId);

            if (verification == null || verification.Status != Models.VerificationStatus.Pending)
                return false;

            verification.Status = Models.VerificationStatus.Rejected;
            verification.ReviewedByUserId = adminUserId;
            verification.ReviewedAt = DateTime.UtcNow;
            verification.AdminNotes = rejectDto.AdminNotes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<StatisticsOverviewDto> GetStatisticsOverviewAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalAgents = await _context.Users.CountAsync(u => u.Role == Models.UserRole.ClearanceAgent);
            var totalCargoOwners = await _context.Users.CountAsync(u => u.Role == Models.UserRole.CargoOwner);
            var totalRequests = await _context.Requests.CountAsync();
            var pendingRequests = await _context.Requests.CountAsync(r => r.Status == Models.RequestStatus.Pending);
            var completedRequests = await _context.Requests.CountAsync(r => r.Status == Models.RequestStatus.Completed);
            var pendingVerifications = await _context.VerificationRequests.CountAsync(v => v.Status == Models.VerificationStatus.Pending);
            var activeAnnouncements = await _context.Announcements.CountAsync(a => a.IsActive);

            return new StatisticsOverviewDto
            {
                TotalUsers = totalUsers,
                TotalAgents = totalAgents,
                TotalCargoOwners = totalCargoOwners,
                TotalRequests = totalRequests,
                PendingRequests = pendingRequests,
                CompletedRequests = completedRequests,
                PendingVerifications = pendingVerifications,
                ActiveAnnouncements = activeAnnouncements
            };
        }

        public async Task<List<DailyStatisticsDto>> GetDailyStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var dates = Enumerable.Range(0, (end - start).Days + 1)
                .Select(offset => start.AddDays(offset).Date)
                .ToList();

            var statistics = new List<DailyStatisticsDto>();

            foreach (var date in dates)
            {
                var nextDate = date.AddDays(1);
                var newUsers = await _context.Users.CountAsync(u => u.CreatedAt >= date && u.CreatedAt < nextDate);
                var newRequests = await _context.Requests.CountAsync(r => r.CreatedAt >= date && r.CreatedAt < nextDate);
                var completedRequests = await _context.Requests.CountAsync(r => r.CompletedAt.HasValue && r.CompletedAt.Value >= date && r.CompletedAt.Value < nextDate);
                var newVerifications = await _context.VerificationRequests.CountAsync(v => v.CreatedAt >= date && v.CreatedAt < nextDate);

                statistics.Add(new DailyStatisticsDto
                {
                    Date = date,
                    NewUsers = newUsers,
                    NewRequests = newRequests,
                    CompletedRequests = completedRequests,
                    NewVerifications = newVerifications
                });
            }

            return statistics;
        }

        public async Task<RequestsReportDto> GetRequestsReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Requests.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CreatedAt <= endDate.Value);

            var requests = await query.ToListAsync();

            var requestsByDate = requests
                .GroupBy(r => r.CreatedAt.Date)
                .Select(g => new RequestByDateDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new RequestsReportDto
            {
                TotalRequests = requests.Count,
                PendingRequests = requests.Count(r => r.Status == Models.RequestStatus.Pending),
                InProgressRequests = requests.Count(r => r.Status == Models.RequestStatus.InProgress),
                CompletedRequests = requests.Count(r => r.Status == Models.RequestStatus.Completed),
                CancelledRequests = requests.Count(r => r.Status == Models.RequestStatus.Cancelled),
                RejectedRequests = requests.Count(r => r.Status == Models.RequestStatus.Rejected),
                RequestsByDate = requestsByDate
            };
        }

        public async Task<UsersReportDto> GetUsersReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Users.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(u => u.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(u => u.CreatedAt <= endDate.Value);

            var users = await query.ToListAsync();

            var usersByDate = users
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new UserByDateDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new UsersReportDto
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive),
                InactiveUsers = users.Count(u => !u.IsActive),
                VerifiedUsers = users.Count(u => u.IsVerified),
                UnverifiedUsers = users.Count(u => !u.IsVerified),
                AgentsCount = users.Count(u => u.Role == Models.UserRole.ClearanceAgent),
                CargoOwnersCount = users.Count(u => u.Role == Models.UserRole.CargoOwner),
                UsersByDate = usersByDate
            };
        }

        public async Task<AnnouncementDto> CreateAnnouncementAsync(int adminUserId, CreateAnnouncementDto createDto)
        {
            var admin = await _context.Users.FindAsync(adminUserId);
            if (admin == null || admin.Role != Models.UserRole.Admin)
                throw new UnauthorizedAccessException("Only admins can create announcements");

            var announcement = new Models.Announcement
            {
                CreatedByUserId = adminUserId,
                Title = createDto.Title,
                Content = createDto.Content,
                Type = createDto.Type,
                IsActive = createDto.IsActive,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return new AnnouncementDto
            {
                Id = announcement.Id,
                CreatedByUserId = announcement.CreatedByUserId,
                CreatedByName = $"{admin.FirstName} {admin.LastName}",
                Title = announcement.Title,
                Content = announcement.Content,
                Type = announcement.Type,
                IsActive = announcement.IsActive,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate,
                CreatedAt = announcement.CreatedAt
            };
        }

        public async Task<ActivityLogListDto> GetActivityLogsAsync(int? userId = null, string? action = null, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.ActivityLogs
                .Include(a => a.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action.Contains(action));

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActivityLogListDto
            {
                Logs = logs.Select(l => new ActivityLogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    UserName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}" : null,
                    Action = l.Action,
                    Description = l.Description,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    IpAddress = l.IpAddress,
                    UserAgent = l.UserAgent,
                    CreatedAt = l.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }
}

