

namespace EventManagementCL.DTO
{
    public class FeedbackSummaryDTO
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
    }
}
