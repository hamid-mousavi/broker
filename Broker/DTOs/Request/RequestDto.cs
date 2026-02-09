using System.ComponentModel.DataAnnotations;
using Broker.Models;

namespace Broker.DTOs.Request
{
    public class RequestDto
    {
        public int Id { get; set; }
        public int CargoOwnerId { get; set; }
        public string CargoOwnerName { get; set; } = string.Empty;
        public int? AgentId { get; set; }
        public string? AgentCompanyName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CargoType { get; set; }
        public string? OriginCountry { get; set; }
        public string? DestinationPort { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string? CustomsCode { get; set; }
        public RequestStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CreateRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CargoType { get; set; }

        [StringLength(50)]
        public string? OriginCountry { get; set; }

        [StringLength(50)]
        public string? DestinationPort { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? EstimatedValue { get; set; }

        [StringLength(50)]
        public string? CustomsCode { get; set; }

        public DateTime? Deadline { get; set; }
    }

    public class UpdateRequestDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? CargoType { get; set; }

        [StringLength(50)]
        public string? OriginCountry { get; set; }

        [StringLength(50)]
        public string? DestinationPort { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? EstimatedValue { get; set; }

        [StringLength(50)]
        public string? CustomsCode { get; set; }

        public DateTime? Deadline { get; set; }
    }

    public class AssignAgentDto
    {
        [Required]
        public int AgentId { get; set; }
    }

    public class UpdateRequestStatusDto
    {
        [Required]
        public RequestStatus Status { get; set; }
    }

    public class RequestSearchDto
    {
        public RequestStatus? Status { get; set; }
        public string? CargoType { get; set; }
        public string? OriginCountry { get; set; }
        public string? DestinationPort { get; set; }
        public int? CargoOwnerId { get; set; }
        public int? AgentId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RequestListResponseDto
    {
        public List<RequestDto> Requests { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}


