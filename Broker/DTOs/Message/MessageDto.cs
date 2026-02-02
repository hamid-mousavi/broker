using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string RequestTitle { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMessageDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class MessageSearchDto
    {
        public int? RequestId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public bool? IsRead { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class MessageListResponseDto
    {
        public List<MessageDto> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int UnreadCount { get; set; }
    }
}

