using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationListDto
    {
        public List<NotificationDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }

    public class NotificationPreferencesDto
    {
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool RequestNotifications { get; set; } = true;
        public bool MessageNotifications { get; set; } = true;
        public bool SystemNotifications { get; set; } = true;
    }
}

