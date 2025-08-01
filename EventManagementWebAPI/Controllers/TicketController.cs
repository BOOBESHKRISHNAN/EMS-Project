using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Services;
using Microsoft.AspNetCore.Authorization;      
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;                 

namespace EventManagementWebAPI.Controllers
{
    [ApiController]                              // simplifies routing for api requests
    [Route("api/[controller]")]                  // api/Ticket
    [Authorize(Roles = "RegisteredUser")]
    public class TicketController : ControllerBase
    {
        //Injects ticket repository and email notification service
        private readonly ITicketRepository _ticketRepo;
        private readonly NotificationService _notificationService;

        public TicketController(ITicketRepository ticketRepo, NotificationService notificationService)
        {
            _ticketRepo = ticketRepo;
            _notificationService = notificationService;
        }

        [HttpPost("book")]
        //IActionResult-returns an action result (e.g., Ok, BadRequest)
        public async Task<IActionResult> BookTicket([FromBody] BookTicketViewModel model) // [FromBody]-booking data is expected in the request body and is bound to the model object
        {
            // null or 0 or negative-invalid
            if (model == null || model.EventID <= 0)
                return BadRequest("Invalid EventID.");

            //User: Refers to the authenticated user making the request.
            var userIdClaim = User.FindFirstValue("user_id");          //FindFirstValue("user_id"): Retrieves the user_id claim from the user's JWT token
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))   //If Claimed user id is null or not int
                return Unauthorized("User ID missing or invalid in token.");

            var ticket = await _ticketRepo.BookTicketAsync(userId, model.EventID, model.NumberOfTickets);     //go to Ticket Repository
            await _notificationService.SendTicketBookingEmail(ticket);
            return ticket != null
                ? Ok(new { ticket.TicketID })
                : NotFound("Event not found.");
        }

        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirstValue("user_id");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID missing or invalid in token.");

            var tickets = await _ticketRepo.GetUserTicketsAsync(userId);
            //Converts tickets to TicketResponseViewModel format for API response
            var response = tickets.Select(t => new TicketResponseViewModel
            {
                TicketID = t.TicketID,
                EventName = t.Event?.Title ?? "Unknown Event",
                BookingDate = t.BookingDate,
                Status = t.Status.ToString()
            });
            return Ok(response);
        }

        [HttpPut("cancel/{ticketId}")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue("user_id");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID missing or invalid in token.");

            var success = await _ticketRepo.CancelTicketAsync(ticketId, userId);
            await _notificationService.SendTicketCancellationEmail(ticketId, userId);
            return success
                ? Ok("Ticket canceled successfully.")
                : BadRequest("Ticket not found or already canceled.");
        }
    }
}
