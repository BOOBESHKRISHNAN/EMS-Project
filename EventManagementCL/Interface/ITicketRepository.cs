//An interface in C# is a blueprint for a class. It contains method signatures but no implementation. Think of it as saying, “Whoever implements me must provide these methods.”
//Abstraction:what should be done, but not how, Loose Coupling, Polymorphism:Multiple classes can implement the same interface in their own way.
using EventManagementCL.Models;
namespace EventManagementCL.Interface
{
    public interface ITicketRepository
    {
        //Task<T> - asynchronous(avoid blocking threads, website responsive)
        Task<Ticket?> BookTicketAsync(int userId, int eventId, int numberOfTickets); //book a ticket for a user for that event
        Task<IEnumerable<Ticket>> GetUserTicketsAsync(int userId); 
        Task<bool> CancelTicketAsync(int ticketId, int userId);
        Task<Ticket?> GetTicketByIdAsync(int ticketId, int userId);
    }
}
