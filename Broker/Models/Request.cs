using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CargoOwnerId { get; set; }

        public int? AgentId { get; set; }

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

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedValue { get; set; }

        [StringLength(50)]
        public string? CustomsCode { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime? Deadline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CargoOwnerId")]
        public CargoOwner CargoOwner { get; set; } = null!;

        [ForeignKey("AgentId")]
        public ClearanceAgent? Agent { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

    public enum RequestStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        Rejected = 5
    }
}
