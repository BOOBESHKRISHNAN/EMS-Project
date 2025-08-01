
using System.ComponentModel.DataAnnotations;


namespace EventManagementCL.DTO
{
    public class FeedbackCreateDTO
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int TicketID { get; set; }

        [Required]
        public int UserId { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }

        [MaxLength(255)]
        public string? Comments { get; set; }
    }
}
