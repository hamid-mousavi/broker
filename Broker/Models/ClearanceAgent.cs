using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class ClearanceAgent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

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
        public string? Website { get; set; }

        public int YearsOfExperience { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; } = 0;

        public int TotalRatings { get; set; } = 0;

        public int CompletedRequests { get; set; } = 0;

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<AgentSpecialization> Specializations { get; set; } = new List<AgentSpecialization>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }

    public class AgentSpecialization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        [StringLength(100)]
        public string SpecializationName { get; set; } = string.Empty;

        [ForeignKey("AgentId")]
        public ClearanceAgent Agent { get; set; } = null!;
    }
}
