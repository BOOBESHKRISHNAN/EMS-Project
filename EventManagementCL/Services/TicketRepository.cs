using EventManagementCL.Data;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using Microsoft.EntityFrameworkCore; //Core EF library for querying and managing data asynchronously.

namespace EventManagementCL.Services
{
    //implements business logic
    public class TicketRepository : ITicketRepository     //TicketRepository is the concrete class that provides real functionality for the methods declared in the interface ITicketRepository
    {
        //Uses constructor injection to get AppDbContext, enabling access to the database
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket?> BookTicketAsync(int userId, int eventId, int numberOfTickets)
        {
            // Check if event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists) return null;

            // Create new ticket
            var ticket = new Ticket
            {
                EventID = eventId,
                UserID = userId,
                BookingDate = DateTime.UtcNow,
                Status = TicketStatus.Payment_pending,
                NumberOfTickets = numberOfTickets
            };

            // Save the ticket
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Reload the ticket with User and Event included for email
            var enrichedTicket = await _context.Tickets
                .Include(t => t.User)    //eagerloading- first itself data loaded
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.TicketID == ticket.TicketID); 
            return enrichedTicket;
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(int userId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Where(t => t.UserID == userId)  //filter tickets by userid
                .ToListAsync();
        }

        public async Task<bool> CancelTicketAsync(int ticketId, int userId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketID == ticketId && t.UserID == userId);    //find the ticket by its TicketID and UserID

            if (ticket == null || ticket.Status == TicketStatus.Cancelled)
                return false;

            ticket.Status = TicketStatus.Cancelled;          //Updates the status to Cancelled, saves changes, and returns true if successful
            _context.Tickets.Update(ticket);
            //returns true if changes were successfully saved, false if nothing happened - why used? because mostly CancelTicket needs to return bool value.
            return await _context.SaveChangesAsync() > 0;   //SaveChanges returns int value—the number of affected rows in the database -Did at least one row get modified? 
        }

        public async Task<Ticket?> GetTicketByIdAsync(int ticketId, int userId)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.TicketID == ticketId && t.UserID == userId);
        }
    }
}
