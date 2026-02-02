using Broker.DTOs.Agent;

namespace Broker.Services
{
    public interface IAgentService
    {
        Task<AgentProfileDto?> GetAgentProfileAsync(int agentId);
        Task<AgentProfileDto?> GetAgentProfileByUserIdAsync(int userId);
        Task<AgentProfileDto?> CreateAgentProfileAsync(int userId, CreateAgentProfileDto createDto);
        Task<AgentProfileDto?> UpdateAgentProfileAsync(int agentId, UpdateAgentProfileDto updateDto);
        Task<AgentListResponseDto> SearchAgentsAsync(AgentSearchDto searchDto);
        Task<bool> DeleteAgentProfileAsync(int agentId);
        Task<bool> VerifyAgentAsync(int agentId);
    }
}

