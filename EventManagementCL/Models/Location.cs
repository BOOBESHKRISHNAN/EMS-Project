

namespace EventManagementCL.Models
{
    public class Location
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Address { get; set; }

        public required string City { get; set; }

        public required string State { get; set; }

        public string? ZipCode { get; set; }

        public decimal VenueFee { get; set; }


        public ICollection<Event>? Events { get; set; }
    }
}
