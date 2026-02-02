namespace Broker.DTOs.Admin
{
    public class VerificationRequestDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string AgentCompanyName { get; set; } = string.Empty;
        public string AgentEmail { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AdminNotes { get; set; }
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ApproveVerificationDto
    {
        public string? AdminNotes { get; set; }
    }

    public class RejectVerificationDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string AdminNotes { get; set; } = string.Empty;
    }
}

