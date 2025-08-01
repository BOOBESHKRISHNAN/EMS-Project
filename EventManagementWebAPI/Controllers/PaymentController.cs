using EventManagementCL.DTO;
using EventManagementCL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepo;

        public PaymentController(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

    
        [HttpPost("venue/{eventId}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> PayVenueFee(int eventId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var payment = await _paymentRepo.RecordVenuePaymentAsync(eventId, email);

            if (payment == null) return NotFound("Event not found.");

            var response = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                Type = payment.Type,
                Amount = payment.Amount,
                PaidBy = payment.PaidBy,
                PaymentDate = payment.PaymentDate
            };

            return Ok(response);
        }


        [HttpPost("ticket/{ticketId}")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> PayTicketFee(int ticketId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var payment = await _paymentRepo.RecordTicketPaymentAsync(ticketId, email);

            if (payment == null) return NotFound("Ticket not found.");

            var response = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                Type = payment.Type,
                Amount = payment.Amount,
                PaidBy = payment.PaidBy,
                PaymentDate = payment.PaymentDate
            };

            return Ok(response);
        }

    }

}