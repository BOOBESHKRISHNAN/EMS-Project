

namespace EventManagementCL.Models //namespaces are used to logically group related code elements like classes, structs, interfaces, enums and Namespaces help organize code into logical groups, making it easier to understand, maintain, and reuse
{
    public enum TicketStatus     //define set of named constants
    {
        Confirmed,              //Internally, enums are stored as integers (e.g., 0, 1), making them memory-efficient.
        Cancelled,
        Payment_pending
    }
    public class Ticket      //blueprint for a ticket object
    {
        public int TicketID { get; set; }   //primary key
        public int EventID { get; set; }     //relationship b/w ticket and an event
        public Event? Event { get; set; } = null!; //null! tells the compiler to assume it's not null
        public int UserID { get; set; }          //relationship b/w ticket and a user
        public User? User { get; set; } = null!; //navigation property- mapping and easier access
        public DateTime BookingDate { get; set; }  
        public TicketStatus Status { get; set; } = TicketStatus.Confirmed;//Indicates the current state of the ticket-by default Confirmed
        public int NumberOfTickets { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>(); //Holds a list of feedbacks associated with this ticket. ICollection<T> is a flexible interface for storing multiple items. Starts as an empty list(new List<Feedback>()), ready to accept feedback from users.
    }
}
