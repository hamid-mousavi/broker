using Broker.Data;
using Broker.DTOs.Message;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MessageDto?> GetMessageByIdAsync(int messageId, int userId)
        {
            var message = await _context.Messages
                .Include(m => m.Request)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == messageId &&
                    (m.SenderId == userId || m.ReceiverId == userId));

            if (message == null) return null;

            return MapToDto(message);
        }

        public async Task<MessageListResponseDto> GetMessagesAsync(MessageSearchDto searchDto, int userId)
        {
            var query = _context.Messages
                .Include(m => m.Request)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .AsQueryable();

            if (searchDto.RequestId.HasValue)
                query = query.Where(m => m.RequestId == searchDto.RequestId.Value);

            if (searchDto.SenderId.HasValue)
                query = query.Where(m => m.SenderId == searchDto.SenderId.Value);

            if (searchDto.ReceiverId.HasValue)
                query = query.Where(m => m.ReceiverId == searchDto.ReceiverId.Value);

            if (searchDto.IsRead.HasValue)
                query = query.Where(m => m.IsRead == searchDto.IsRead.Value);

            var totalCount = await query.CountAsync();

            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var unreadCount = await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);

            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            return new MessageListResponseDto
            {
                Messages = messages.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                UnreadCount = unreadCount
            };
        }

        public async Task<MessageListResponseDto> GetConversationAsync(int requestId, int otherUserId, int userId, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.Messages
                .Include(m => m.Request)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.RequestId == requestId &&
                    ((m.SenderId == userId && m.ReceiverId == otherUserId) ||
                     (m.SenderId == otherUserId && m.ReceiverId == userId)))
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var messages = await query
                .OrderBy(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new MessageListResponseDto
            {
                Messages = messages.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                UnreadCount = 0
            };
        }

        public async Task<MessageDto?> SendMessageAsync(int senderId, CreateMessageDto createDto)
        {
            // Verify request exists and user has access
            var request = await _context.Requests
                .Include(r => r.CargoOwner)
                .Include(r => r.Agent)
                .FirstOrDefaultAsync(r => r.Id == createDto.RequestId);

            if (request == null) return null;

            // Check if sender has permission (must be cargo owner or assigned agent)
            var hasPermission = request.CargoOwner.UserId == senderId ||
                               (request.AgentId.HasValue && request.Agent!.UserId == senderId);

            if (!hasPermission) return null;

            // Verify receiver has access to the request
            var receiver = await _context.Users.FindAsync(createDto.ReceiverId);
            if (receiver == null) return null;

            var receiverHasAccess = request.CargoOwner.UserId == createDto.ReceiverId ||
                                    (request.AgentId.HasValue && request.Agent!.UserId == createDto.ReceiverId);

            if (!receiverHasAccess) return null;

            var message = new Models.Message
            {
                RequestId = createDto.RequestId,
                SenderId = senderId,
                ReceiverId = createDto.ReceiverId,
                Content = createDto.Content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return await GetMessageByIdAsync(message.Id, senderId);
        }

        public async Task<bool> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

            if (message == null || message.IsRead) return false;

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkConversationAsReadAsync(int requestId, int userId, int otherUserId)
        {
            var messages = await _context.Messages
                .Where(m => m.RequestId == requestId &&
                    m.ReceiverId == userId &&
                    m.SenderId == otherUserId &&
                    !m.IsRead)
                .ToListAsync();

            if (!messages.Any()) return false;

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadMessageCountAsync(int userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        private MessageDto MapToDto(Models.Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                RequestId = message.RequestId,
                RequestTitle = message.Request.Title,
                SenderId = message.SenderId,
                SenderName = $"{message.Sender.FirstName} {message.Sender.LastName}",
                ReceiverId = message.ReceiverId,
                ReceiverName = $"{message.Receiver.FirstName} {message.Receiver.LastName}",
                Content = message.Content,
                IsRead = message.IsRead,
                ReadAt = message.IsRead ? message.ReadAt : null,
                CreatedAt = message.CreatedAt
            };
        }
    }
}


