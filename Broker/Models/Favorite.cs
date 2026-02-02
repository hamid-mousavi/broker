using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CargoOwnerId { get; set; }

        [Required]
        public int AgentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("CargoOwnerId")]
        public CargoOwner CargoOwner { get; set; } = null!;

        [ForeignKey("AgentId")]
        public ClearanceAgent Agent { get; set; } = null!;
    }
}

