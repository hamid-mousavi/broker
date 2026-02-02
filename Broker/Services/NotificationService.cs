using Broker.Data;
using Broker.DTOs.Notification;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationListDto> GetUserNotificationsAsync(int userId, bool? unreadOnly = false)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .AsQueryable();

            if (unreadOnly == true)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var unreadCount = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return new NotificationListDto
            {
                Notifications = notifications.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                }).ToList(),
                TotalCount = notifications.Count,
                UnreadCount = unreadCount
            };
        }

        public async Task<NotificationDto?> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null) return null;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Link = notification.Link,
                IsRead = notification.IsRead,
                ReadAt = notification.ReadAt,
                CreatedAt = notification.CreatedAt
            };
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateNotificationPreferencesAsync(int userId, NotificationPreferencesDto preferences)
        {
            // This would typically be stored in a separate table or user settings
            // For now, we'll just log it as an activity
            var activityLog = new Models.ActivityLog
            {
                UserId = userId,
                Action = "UpdateNotificationPreferences",
                Description = $"User updated notification preferences",
                EntityType = "User",
                EntityId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

