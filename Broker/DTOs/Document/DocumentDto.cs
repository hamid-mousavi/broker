using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Document
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public long FileSize { get; set; }
        public string? Description { get; set; }
        public bool IsVerified { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UploadDocumentDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class DocumentTypeDto
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

