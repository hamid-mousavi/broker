using Broker.Data;
using Broker.DTOs.Agent;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class AgentService : IAgentService
    {
        private readonly ApplicationDbContext _context;

        public AgentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AgentProfileDto?> GetAgentProfileAsync(int agentId)
        {
            var agent = await _context.ClearanceAgents
                .Include(a => a.User)
                .Include(a => a.Specializations)
                .FirstOrDefaultAsync(a => a.Id == agentId);

            if (agent == null) return null;

            return MapToDto(agent);
        }

        public async Task<AgentProfileDto?> GetAgentProfileByUserIdAsync(int userId)
        {
            var agent = await _context.ClearanceAgents
                .Include(a => a.User)
                .Include(a => a.Specializations)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (agent == null) return null;

            return MapToDto(agent);
        }

        public async Task<AgentProfileDto?> CreateAgentProfileAsync(int userId, CreateAgentProfileDto createDto)
        {
            // Check if user exists and is a ClearanceAgent
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.Role == Models.UserRole.ClearanceAgent);

            if (user == null) return null;

            // Check if profile already exists
            if (await _context.ClearanceAgents.AnyAsync(a => a.UserId == userId))
                return null;

            var agent = new Models.ClearanceAgent
            {
                UserId = userId,
                CompanyName = createDto.CompanyName,
                LicenseNumber = createDto.LicenseNumber,
                Description = createDto.Description,
                Address = createDto.Address,
                City = createDto.City,
                Province = createDto.Province,
                PostalCode = createDto.PostalCode,
                Website = createDto.Website,
                YearsOfExperience = createDto.YearsOfExperience,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClearanceAgents.Add(agent);
            await _context.SaveChangesAsync();

            // Add specializations
            if (createDto.Specializations != null && createDto.Specializations.Any())
            {
                foreach (var spec in createDto.Specializations)
                {
                    _context.AgentSpecializations.Add(new Models.AgentSpecialization
                    {
                        AgentId = agent.Id,
                        SpecializationName = spec
                    });
                }
                await _context.SaveChangesAsync();
            }

            return await GetAgentProfileAsync(agent.Id);
        }

        public async Task<AgentProfileDto?> UpdateAgentProfileAsync(int agentId, UpdateAgentProfileDto updateDto)
        {
            var agent = await _context.ClearanceAgents
                .Include(a => a.Specializations)
                .FirstOrDefaultAsync(a => a.Id == agentId);

            if (agent == null) return null;

            if (!string.IsNullOrWhiteSpace(updateDto.CompanyName))
                agent.CompanyName = updateDto.CompanyName;

            if (updateDto.LicenseNumber != null)
                agent.LicenseNumber = updateDto.LicenseNumber;

            if (updateDto.Description != null)
                agent.Description = updateDto.Description;

            if (updateDto.Address != null)
                agent.Address = updateDto.Address;

            if (updateDto.PersonalAddress != null)
                agent.PersonalAddress = updateDto.PersonalAddress;

            if (updateDto.LegalAddress != null)
                agent.LegalAddress = updateDto.LegalAddress;

            if (updateDto.City != null)
                agent.City = updateDto.City;

            if (updateDto.Province != null)
                agent.Province = updateDto.Province;

            if (updateDto.PostalCode != null)
                agent.PostalCode = updateDto.PostalCode;

            if (updateDto.Website != null)
                agent.Website = updateDto.Website;

            if (updateDto.YearsOfExperience.HasValue)
                agent.YearsOfExperience = updateDto.YearsOfExperience.Value;

            if (updateDto.IsLegalEntity.HasValue)
                agent.IsLegalEntity = updateDto.IsLegalEntity.Value;

            if (updateDto.NationalId != null)
                agent.NationalId = updateDto.NationalId;

            if (updateDto.RegistrationNumber != null)
                agent.RegistrationNumber = updateDto.RegistrationNumber;

            if (updateDto.EconomicCode != null)
                agent.EconomicCode = updateDto.EconomicCode;

            agent.UpdatedAt = DateTime.UtcNow;

            // Update specializations
            if (updateDto.Specializations != null)
            {
                // Remove existing specializations
                var existingSpecs = agent.Specializations.ToList();
                _context.AgentSpecializations.RemoveRange(existingSpecs);

                // Add new specializations
                foreach (var spec in updateDto.Specializations)
                {
                    _context.AgentSpecializations.Add(new Models.AgentSpecialization
                    {
                        AgentId = agent.Id,
                        SpecializationName = spec
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await GetAgentProfileAsync(agentId);
        }

        public async Task<AgentListResponseDto> SearchAgentsAsync(AgentSearchDto searchDto)
        {
            var query = _context.ClearanceAgents
                .Include(a => a.User)
                .Include(a => a.Specializations)
                .AsQueryable();

            // Only active users should be visible in public lists
            query = query.Where(a => a.User.IsActive);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchDto.City))
                query = query.Where(a => a.City != null && a.City.Contains(searchDto.City));

            if (!string.IsNullOrWhiteSpace(searchDto.Province))
                query = query.Where(a => a.Province != null && a.Province.Contains(searchDto.Province));

            if (!string.IsNullOrWhiteSpace(searchDto.Specialization))
                query = query.Where(a => a.Specializations.Any(s => s.SpecializationName.Contains(searchDto.Specialization)));

            if (searchDto.MinYearsOfExperience.HasValue)
                query = query.Where(a => a.YearsOfExperience >= searchDto.MinYearsOfExperience.Value);

            if (searchDto.MinRating.HasValue)
                query = query.Where(a => a.AverageRating >= searchDto.MinRating.Value);

            if (searchDto.IsVerified.HasValue)
                query = query.Where(a => a.IsVerified == searchDto.IsVerified.Value);
            else
                query = query.Where(a => a.IsVerified);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.CompanyName.ToLower().Contains(searchTerm) ||
                    (a.Description != null && a.Description.ToLower().Contains(searchTerm)) ||
                    (a.City != null && a.City.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            // Apply pagination
            var agents = await query
                .OrderByDescending(a => a.AverageRating)
                .ThenByDescending(a => a.CompletedRequests)
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var userIds = agents.Select(a => a.UserId).ToList();
            var verifiedDocUserIds = await _context.Documents
                .Where(d => userIds.Contains(d.UserId) && d.Status == Models.DocumentVerificationStatus.Approved)
                .Select(d => d.UserId)
                .Distinct()
                .ToListAsync();
            var verifiedDocSet = verifiedDocUserIds.ToHashSet();

            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            return new AgentListResponseDto
            {
                Agents = agents.Select(a =>
                {
                    var dto = MapToDto(a);
                    dto.HasVerifiedDocuments = verifiedDocSet.Contains(a.UserId);
                    return dto;
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<bool> DeleteAgentProfileAsync(int agentId)
        {
            var agent = await _context.ClearanceAgents.FindAsync(agentId);
            if (agent == null) return false;

            _context.ClearanceAgents.Remove(agent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyAgentAsync(int agentId)
        {
            var agent = await _context.ClearanceAgents.FindAsync(agentId);
            if (agent == null) return false;

            agent.IsVerified = true;
            agent.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private AgentProfileDto MapToDto(Models.ClearanceAgent agent)
        {
            return new AgentProfileDto
            {
                Id = agent.Id,
                UserId = agent.UserId,
                CompanyName = agent.CompanyName,
                LicenseNumber = agent.LicenseNumber,
                Description = agent.Description,
                Address = agent.Address,
                PersonalAddress = agent.PersonalAddress,
                LegalAddress = agent.LegalAddress,
                City = agent.City,
                Province = agent.Province,
                PostalCode = agent.PostalCode,
                Website = agent.Website,
                YearsOfExperience = agent.YearsOfExperience,
                AverageRating = agent.AverageRating,
                TotalRatings = agent.TotalRatings,
                CompletedRequests = agent.CompletedRequests,
                IsVerified = agent.IsVerified,
                IsLegalEntity = agent.IsLegalEntity,
                NationalId = agent.NationalId,
                RegistrationNumber = agent.RegistrationNumber,
                EconomicCode = agent.EconomicCode,
                PhoneNumber = agent.User?.PhoneNumber,
                HasVerifiedDocuments = false,
                Specializations = agent.Specializations.Select(s => s.SpecializationName).ToList(),
                CreatedAt = agent.CreatedAt
            };
        }
    }
}


