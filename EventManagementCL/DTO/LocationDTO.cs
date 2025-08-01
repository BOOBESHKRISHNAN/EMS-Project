

namespace EventManagementCL.DTO
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public required List<EventPreviewDTO> Events { get; set; }
    }
}
