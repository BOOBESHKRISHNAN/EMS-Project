

namespace EventManagementCL.Models
{
   public class Event
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int LocationId { get; set; }

        public Location? Location { get; set; }
        public required int OrganizerId { get; set; } // FK to User
        public User? Organizer { get; set; }

        public decimal TicketPrice { get; set; }


        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        
    }
}
