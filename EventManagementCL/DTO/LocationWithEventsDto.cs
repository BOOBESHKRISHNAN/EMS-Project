

namespace EventManagementCL.DTO
{
    public class LocationWithEventsDto

    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public decimal VenueFee { get; set; }
        public required List<EventDto> Events { get; set; }

    }

}
