using Broker.Data;
using Broker.DTOs.Request;
using Broker.Models;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;

        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RequestDto?> GetRequestByIdAsync(int requestId, int? userId = null)
        {
            var query = _context.Requests
                .Include(r => r.CargoOwner)
                    .ThenInclude(co => co != null ? co.User : null!)
                .Include(r => r.Agent)
                    .ThenInclude(a => a != null ? a.User : null!)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(r =>
                    r.CargoOwnerId == userId.Value ||
                    (r.AgentId.HasValue && r.Agent != null && r.Agent.UserId == userId.Value));
            }

            var request = await query.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null) return null;

            return MapToDto(request);
        }

        public async Task<RequestListResponseDto> GetRequestsAsync(RequestSearchDto searchDto, int? userId = null)
        {
            var query = _context.Requests
                .Include(r => r.CargoOwner)
                    .ThenInclude(co => co != null ? co.User : null!)
                .Include(r => r.Agent)
                    .ThenInclude(a => a != null ? a.User : null!)
                .AsQueryable();

            // Apply filters
            if (searchDto.Status.HasValue)
                query = query.Where(r => r.Status == searchDto.Status.Value);

            if (!string.IsNullOrWhiteSpace(searchDto.CargoType))
                query = query.Where(r => r.CargoType != null && r.CargoType.Contains(searchDto.CargoType));

            if (!string.IsNullOrWhiteSpace(searchDto.OriginCountry))
                query = query.Where(r => r.OriginCountry != null && r.OriginCountry.Contains(searchDto.OriginCountry));

            if (!string.IsNullOrWhiteSpace(searchDto.DestinationPort))
                query = query.Where(r => r.DestinationPort != null && r.DestinationPort.Contains(searchDto.DestinationPort));

            if (searchDto.CargoOwnerId.HasValue)
                query = query.Where(r => r.CargoOwnerId == searchDto.CargoOwnerId.Value);

            if (searchDto.AgentId.HasValue)
                query = query.Where(r => r.AgentId == searchDto.AgentId.Value);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.Title.ToLower().Contains(searchTerm) ||
                    (r.Description != null && r.Description.ToLower().Contains(searchTerm)) ||
                    (r.CargoType != null && r.CargoType.ToLower().Contains(searchTerm)));
            }

            if (userId.HasValue)
            {
                query = query.Where(r =>
                    r.CargoOwnerId == userId.Value ||
                    (r.AgentId.HasValue && r.Agent != null && r.Agent.UserId == userId.Value));
            }

            var totalCount = await query.CountAsync();

            var requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            return new RequestListResponseDto
            {
                Requests = requests.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<RequestDto?> CreateRequestAsync(int cargoOwnerId, CreateRequestDto createDto)
        {
            var cargoOwner = await _context.CargoOwners
                .FirstOrDefaultAsync(co => co.Id == cargoOwnerId);

            if (cargoOwner == null) return null;

            var request = new Request
            {
                CargoOwnerId = cargoOwnerId,
                Title = createDto.Title,
                Description = createDto.Description,
                CargoType = createDto.CargoType,
                OriginCountry = createDto.OriginCountry,
                DestinationPort = createDto.DestinationPort,
                EstimatedValue = createDto.EstimatedValue,
                CustomsCode = createDto.CustomsCode,
                Status = RequestStatus.Pending,
                Deadline = createDto.Deadline,
                CreatedAt = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return await GetRequestByIdAsync(request.Id);
        }

        public async Task<RequestDto?> UpdateRequestAsync(int requestId, UpdateRequestDto updateDto, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.CargoOwner)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.CargoOwner.UserId != userId)
                return null;

            if (!string.IsNullOrWhiteSpace(updateDto.Title))
                request.Title = updateDto.Title;

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
                request.Description = updateDto.Description;

            if (updateDto.CargoType != null)
                request.CargoType = updateDto.CargoType;

            if (updateDto.OriginCountry != null)
                request.OriginCountry = updateDto.OriginCountry;

            if (updateDto.DestinationPort != null)
                request.DestinationPort = updateDto.DestinationPort;

            if (updateDto.EstimatedValue.HasValue)
                request.EstimatedValue = updateDto.EstimatedValue;

            if (updateDto.CustomsCode != null)
                request.CustomsCode = updateDto.CustomsCode;

            if (updateDto.Deadline.HasValue)
                request.Deadline = updateDto.Deadline;

            request.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetRequestByIdAsync(requestId);
        }

        public async Task<bool> DeleteRequestAsync(int requestId, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.CargoOwner)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.CargoOwner.UserId != userId)
                return false;

            if (request.Status == RequestStatus.InProgress || request.Status == RequestStatus.Completed)
                return false; // Cannot delete in-progress or completed requests

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RequestDto?> AssignAgentAsync(int requestId, AssignAgentDto assignDto, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.CargoOwner)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.CargoOwner.UserId != userId)
                return null;

            var agent = await _context.ClearanceAgents
                .FirstOrDefaultAsync(a => a.Id == assignDto.AgentId && a.IsVerified);

            if (agent == null) return null;

            request.AgentId = assignDto.AgentId;
            request.Status = RequestStatus.InProgress;
            request.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetRequestByIdAsync(requestId);
        }

        public async Task<RequestDto?> UpdateRequestStatusAsync(int requestId, UpdateRequestStatusDto statusDto, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.CargoOwner)
                .Include(r => r.Agent)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return null;

            // Check permissions
            var canUpdate = request.CargoOwner.UserId == userId ||
                           (request.AgentId.HasValue && request.Agent!.UserId == userId);

            if (!canUpdate) return null;

            request.Status = statusDto.Status;
            request.UpdatedAt = DateTime.UtcNow;

            if (statusDto.Status == RequestStatus.Completed)
            {
                request.CompletedAt = DateTime.UtcNow;
                if (request.AgentId.HasValue)
                {
                    var agent = await _context.ClearanceAgents.FindAsync(request.AgentId.Value);
                    if (agent != null)
                    {
                        agent.CompletedRequests++;
                        agent.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return await GetRequestByIdAsync(requestId);
        }

        public async Task<RequestListResponseDto> GetMyRequestsAsync(int userId, RequestStatus? status = null, int pageNumber = 1, int pageSize = 10)
        {
            var searchDto = new RequestSearchDto
            {
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await GetRequestsAsync(searchDto, userId);
        }

        private RequestDto MapToDto(Request request)
        {
            return new RequestDto
            {
                Id = request.Id,
                CargoOwnerId = request.CargoOwnerId,
                CargoOwnerName = $"{request.CargoOwner.User.FirstName} {request.CargoOwner.User.LastName}",
                AgentId = request.AgentId,
                AgentCompanyName = request.Agent?.CompanyName,
                Title = request.Title,
                Description = request.Description,
                CargoType = request.CargoType,
                OriginCountry = request.OriginCountry,
                DestinationPort = request.DestinationPort,
                EstimatedValue = request.EstimatedValue,
                CustomsCode = request.CustomsCode,
                Status = request.Status,
                StatusName = request.Status.ToString(),
                Deadline = request.Deadline,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                CompletedAt = request.CompletedAt
            };
        }
    }
}
