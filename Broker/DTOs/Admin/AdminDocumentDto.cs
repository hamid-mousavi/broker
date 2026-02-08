namespace Broker.DTOs.Admin
{
    public class AdminDocumentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? UserPhoneNumber { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsVerified { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
