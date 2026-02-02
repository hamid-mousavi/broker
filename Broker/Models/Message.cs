using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Broker.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestId { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("RequestId")]
        public Request Request { get; set; } = null!;

        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;
    }
}


