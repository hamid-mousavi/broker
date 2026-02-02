using Broker.DTOs.Notification;

namespace Broker.Services
{
    public interface INotificationService
    {
        Task<NotificationListDto> GetUserNotificationsAsync(int userId, bool? unreadOnly = false);
        Task<NotificationDto?> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<bool> UpdateNotificationPreferencesAsync(int userId, NotificationPreferencesDto preferences);
    }
}

