

namespace EventManagementCL.DTO
{
    public class TicketResponseViewModel
    {
        public int TicketID { get; set; }
        public required string EventName { get; set; }
        public DateTime BookingDate { get; set; }
        public required string Status { get; set; }
    }
}
