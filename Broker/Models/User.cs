using System.ComponentModel.DataAnnotations;

namespace Broker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public bool IsVerified { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? ProfileImagePath { get; set; }

        [StringLength(500)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        [StringLength(500)]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        [StringLength(500)]
        public string? EmailVerificationToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ClearanceAgent? ClearanceAgent { get; set; }
        public CargoOwner? CargoOwner { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();
        public ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public enum UserRole
    {
        ClearanceAgent = 1,
        CargoOwner = 2,
        Admin = 3
    }
}
