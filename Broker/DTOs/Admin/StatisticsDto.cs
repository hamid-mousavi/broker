namespace Broker.DTOs.Admin
{
    public class StatisticsOverviewDto
    {
        public int TotalUsers { get; set; }
        public int TotalAgents { get; set; }
        public int TotalCargoOwners { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int PendingVerifications { get; set; }
        public int ActiveAnnouncements { get; set; }
    }

    public class DailyStatisticsDto
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int NewRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int NewVerifications { get; set; }
    }

    public class RequestsReportDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int CancelledRequests { get; set; }
        public int RejectedRequests { get; set; }
        public List<RequestByDateDto> RequestsByDate { get; set; } = new();
    }

    public class RequestByDateDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class UsersReportDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int VerifiedUsers { get; set; }
        public int UnverifiedUsers { get; set; }
        public int AgentsCount { get; set; }
        public int CargoOwnersCount { get; set; }
        public List<UserByDateDto> UsersByDate { get; set; } = new();
    }

    public class UserByDateDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}

