using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;


namespace EventManagementCL.Services
{
    public class NotificationService
    {
        private readonly EmailService _emailService;
        private readonly ITicketRepository _ticketRepo;

        public NotificationService(EmailService emailService, ITicketRepository ticketRepo)
        {
            _emailService = emailService;
            _ticketRepo = ticketRepo;
        }

        public async Task SendUserWelcomeEmail(User user)
        {
            string subject = "Welcome to EventEase!";
            string body = $@"
                <h3>Hi {user.FirstName},</h3>
                <p>Welcome aboard! Your account is now active and ready for use.</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task SendAdminRegistrationEmail(User user)
        {
            string subject = "Admin Access Granted";
            string body = $@"
                <h3>Hello {user.FirstName},</h3>
                <p>You are now registered as an administrator for <b>{user.Location}</b>. Start managing events efficiently!</p>
                <p>Login credentials : </p>
                <p>Email : {user.Email}</p>
                <p>Password : admin@123</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task SendOrganizerWelcomeEmail(User user)
        {
            string subject = "Organizer Access Activated";
            string body = $@"
                <h3>Hey {user.FirstName},</h3>
                <p>Your organizer profile is active. Ready to create amazing events?</p>
                <p>Login credentials : </p>
                <p>Email : {user.Email}</p>
                <p>Password : organizer@123</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }
        public async Task SendEventCreationEmail(User organizer, EventResponseDTO eventInfo)
        {
            string subject = $"Event Created: {eventInfo.Title}";

            string body = $@"
        <h3>Hello {organizer.FirstName},</h3>
        <p>Your event <strong>{eventInfo.Title}</strong> at <strong>{eventInfo.Location.Name}</strong> is scheduled from 
        <b>{eventInfo.StartDate:MMM dd, yyyy hh:mm tt}</b> to <b>{eventInfo.EndDate:MMM dd, yyyy hh:mm tt}</b>.</p>
        <p>Thank you for organizing with us!</p>";

            await _emailService.SendEmailAsync(organizer.Email, subject, body);
        }

        public async Task SendTicketBookingEmail(Ticket ticket)
        {
            if (ticket?.User == null || ticket?.Event == null)
            {
                // Optional: Log or handle this case gracefully
                throw new ArgumentException("Ticket is missing associated User or Event data.");
            }

            string subject = "Ticket Confirmed!";
            string body = $@"
        <h3>Hi {ticket.User.FirstName},</h3>
        <p>Your ticket for <strong>{ticket.Event.Title}</strong> has been confirmed.</p>
        <p>Booking Date: {ticket.BookingDate:MMM dd, yyyy}</p>";

            await _emailService.SendEmailAsync(ticket.User.Email, subject, body);
        }


        public async Task SendTicketCancellationEmail(int ticketId, int userId)
        {
            // You’ll need a method like GetTicketByIdAsync(ticketId, userId) in your repo
            var ticket = await _ticketRepo.GetTicketByIdAsync(ticketId, userId);
            if (ticket == null) return;

            string subject = "Ticket Cancelled";
            string body = $@"
        <h3>Hi {ticket.User.FirstName},</h3>
        <p>Your ticket for <strong>{ticket.Event?.Title}</strong> has been cancelled.</p>
        <p>Cancellation Date: {DateTime.UtcNow:MMM dd, yyyy}</p>";

            await _emailService.SendEmailAsync(ticket.User.Email, subject, body);
        }

    }
}
