using EventManagementCL.Data;
using EventManagementCL.Models;
using EventManagementCL.Interface;
using Microsoft.EntityFrameworkCore;


namespace EventManagementCL.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> RecordVenuePaymentAsync(int eventId, string organizerEmail)
        {
            var ev = await _context.Events
                .Include(e => e.Location)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null || ev.Location == null)
                return null;

            var numDays = (ev.EndDate - ev.StartDate).Days + 1;
            var amount = ev.Location.VenueFee * numDays;

            var payment = new Payment
            {
                EventId = eventId,
                Type = PaymentType.VenueFee,
                Amount = amount,
                PaidBy = organizerEmail,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> RecordTicketPaymentAsync(int ticketId, string userEmail)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.TicketID == ticketId);

            if (ticket == null || ticket.Event == null || ticket.Status == TicketStatus.Confirmed)
                return null;

            var amount = ticket.Event.TicketPrice * ticket.NumberOfTickets;

            var payment = new Payment
            {
                TicketId = ticketId,
                EventId = ticket.Event?.Id,
                Type = PaymentType.TicketFee,
                Amount = amount,
                PaidBy = userEmail,
                PaymentDate = DateTime.UtcNow
            };

            ticket.Status=TicketStatus.Confirmed;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
