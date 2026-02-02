using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}

