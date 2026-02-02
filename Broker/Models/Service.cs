using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [StringLength(50)]
        public string? PriceUnit { get; set; } // Per Item, Per Hour, Fixed, etc.

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("AgentId")]
        public ClearanceAgent Agent { get; set; } = null!;
    }
}

