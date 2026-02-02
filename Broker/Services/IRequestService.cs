using Broker.DTOs.Request;
using Broker.Models;

namespace Broker.Services
{
    public interface IRequestService
    {
        Task<RequestDto?> GetRequestByIdAsync(int requestId, int? userId = null);
        Task<RequestListResponseDto> GetRequestsAsync(RequestSearchDto searchDto, int? userId = null);
        Task<RequestDto?> CreateRequestAsync(int cargoOwnerId, CreateRequestDto createDto);
        Task<RequestDto?> UpdateRequestAsync(int requestId, UpdateRequestDto updateDto, int userId);
        Task<bool> DeleteRequestAsync(int requestId, int userId);
        Task<RequestDto?> AssignAgentAsync(int requestId, AssignAgentDto assignDto, int userId);
        Task<RequestDto?> UpdateRequestStatusAsync(int requestId, UpdateRequestStatusDto statusDto, int userId);
        Task<RequestListResponseDto> GetMyRequestsAsync(int userId, RequestStatus? status = null, int pageNumber = 1, int pageSize = 10);
    }
}


