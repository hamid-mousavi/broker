using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.Admin
{
    public class AnnouncementDto
    {
        public int Id { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAnnouncementDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}

