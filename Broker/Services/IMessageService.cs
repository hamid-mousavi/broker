using Broker.DTOs.Message;

namespace Broker.Services
{
    public interface IMessageService
    {
        Task<MessageDto?> GetMessageByIdAsync(int messageId, int userId);
        Task<MessageListResponseDto> GetMessagesAsync(MessageSearchDto searchDto, int userId);
        Task<MessageListResponseDto> GetConversationAsync(int requestId, int otherUserId, int userId, int pageNumber = 1, int pageSize = 20);
        Task<MessageDto?> SendMessageAsync(int senderId, CreateMessageDto createDto);
        Task<bool> MarkAsReadAsync(int messageId, int userId);
        Task<bool> MarkConversationAsReadAsync(int requestId, int userId, int otherUserId);
        Task<int> GetUnreadMessageCountAsync(int userId);
    }
}

