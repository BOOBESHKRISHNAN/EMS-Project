using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EventManagementCL.Models
{
    public class Feedback
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int FeedbackId { get; set; } // Primary Key

        [Required]
        public int EventId { get; set; } // Foreign Key

        [Required]
        public int TicketID { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign Key

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Comments { get; set; }

        [Required]
        public DateTime SubmittedTimestamp { get; set; }

        // Navigation properties
        public Event? Event { get; set; }
        public User? User { get; set; }
        public Ticket? Ticket { get; set; }
    }
}
