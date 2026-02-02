namespace Broker.DTOs.Admin
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ActivityLogListDto
    {
        public List<ActivityLogDto> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

