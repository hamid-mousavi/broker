using System.ComponentModel.DataAnnotations;
using Broker.Models;

namespace Broker.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public bool IsLegalEntity { get; set; } = false;

        [StringLength(20)]
        public string? NationalId { get; set; }

        [StringLength(50)]
        public string? RegistrationNumber { get; set; }

        [StringLength(50)]
        public string? EconomicCode { get; set; }
    }
}

