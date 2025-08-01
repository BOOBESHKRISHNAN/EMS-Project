

namespace EventManagementCL.DTO
{
    public class LocationCreateDto
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public string? ZipCode { get; set; }
        public decimal VenueFee { get; set; }
    }
}
