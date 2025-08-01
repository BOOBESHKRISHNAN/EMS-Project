using EventManagementCL.Models;


namespace EventManagementCL.Interface
{
    public interface IPaymentRepository
    {
        Task<Payment> RecordVenuePaymentAsync(int eventId, string organizerEmail);
        Task<Payment> RecordTicketPaymentAsync(int ticketId, string userEmail);
    }

}
