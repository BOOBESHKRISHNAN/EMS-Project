using EventManagementCL.Models;


namespace EventManagementCL.DTO
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public PaymentType Type { get; set; }
        public decimal Amount { get; set; }
        public string? PaidBy { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
