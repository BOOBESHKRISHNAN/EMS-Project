

namespace EventManagementCL.Models
{
    public enum PaymentType
    {
        VenueFee,
        TicketFee
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int? EventId { get; set; }
        public int? TicketId { get; set; }
        public PaymentType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? PaidBy { get; set; } // Organizer or User Email
    }

}
