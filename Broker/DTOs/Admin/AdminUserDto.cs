namespace Broker.DTOs.Admin
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AdminUserListDto
    {
        public List<AdminUserDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class UpdateUserStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class UpdateAdminUserDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class UpdateUserVerificationDto
    {
        public bool IsVerified { get; set; }
    }

    public class AdminResetPasswordDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}

