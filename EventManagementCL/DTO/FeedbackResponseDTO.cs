

namespace EventManagementCL.DTO
{
    public class FeedbackResponseDTO
    {
        public int FeedbackId { get; set; }
        public int EventId { get; set; }
        public int TicketID { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }
        public DateTime SubmittedTimestamp { get; set; }
        public string? EventName { get; set; }
    }
}
