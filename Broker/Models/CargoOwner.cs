using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class CargoOwner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(20)]
        public string? NationalId { get; set; }

        [StringLength(20)]
        public string? EconomicCode { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Province { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
