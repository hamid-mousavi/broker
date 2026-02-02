using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Agent
{
    public class AgentProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Website { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int CompletedRequests { get; set; }
        public bool IsVerified { get; set; }
        public List<string> Specializations { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAgentProfileDto
    {
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? LicenseNumber { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Province { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        [Url]
        public string? Website { get; set; }

        [Range(0, 50)]
        public int YearsOfExperience { get; set; }

        public List<string> Specializations { get; set; } = new();
    }

    public class UpdateAgentProfileDto
    {
        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(20)]
        public string? LicenseNumber { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Province { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        [Url]
        public string? Website { get; set; }

        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }

        public List<string>? Specializations { get; set; }
    }

    public class AgentSearchDto
    {
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Specialization { get; set; }
        public int? MinYearsOfExperience { get; set; }
        public decimal? MinRating { get; set; }
        public bool? IsVerified { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AgentListResponseDto
    {
        public List<AgentProfileDto> Agents { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}


