

namespace EventManagementCL.DTO
{
    public class EventDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LocationId { get; set; }
        public string? Location { get; set; }
        public string? OrganizerName { get; set; }
    }
}
