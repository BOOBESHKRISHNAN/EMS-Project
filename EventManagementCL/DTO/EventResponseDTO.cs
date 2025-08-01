
namespace EventManagementCL.DTO
{
    public class EventResponseDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required int LocationId { get; set; }

        public required string OrganizerName { get; set; }
        public required EventWithLocationsDTO Location { get; set; }
    }
}
