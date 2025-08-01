

namespace EventManagementCL.DTO
{
    public class EventCreateDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int LocationId { get; set; }
        public string? Location { get; internal set; }

        public decimal TicketPrice { get; set; }
    }
}
