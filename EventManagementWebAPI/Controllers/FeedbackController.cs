using EventManagementCL.DTO;
using EventManagementCL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepo;

        public FeedbackController(IFeedbackRepository feedbackRepo)
        {
            _feedbackRepo = feedbackRepo;
        }

        // GET: api/Feedback — All feedbacks (Admin, Organizer, User)
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Organizer,RegisteredUser")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackRepo.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }


        // GET: api/Feedback/summary
        [HttpGet("summary")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetEventFeedbackSummaries()
        {
            var summary = await _feedbackRepo.GetEventRatingsSummaryAsync();
            return Ok(summary);
        }

        // POST: api/Feedback — Submit feedback
        [HttpPost]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDTO feedbackDto)
        {
            var createdFeedback = await _feedbackRepo.CreateFeedbackAsync(feedbackDto);
            if (createdFeedback == null)
                return BadRequest("Feedback not allowed. You must hold a valid ticket for the event.");

            return Ok(new
            {
                Message = "Feedback submitted successfully.",
                FeedbackId = createdFeedback.FeedbackId,
                EventName = createdFeedback.EventName,
                Rating = createdFeedback.Rating,
                SubmittedTimestamp = createdFeedback.SubmittedTimestamp
            });
        }

        // PUT: api/Feedback/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackCreateDTO feedbackDto)
        {
            var updated = await _feedbackRepo.UpdateFeedbackAsync(id, feedbackDto);
            return updated
                ? Ok("Feedback updated successfully.")
                : BadRequest("Feedback not found or update failed.");
        }

        // DELETE: api/Feedback/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,RegisteredUser")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var deleted = await _feedbackRepo.DeleteFeedbackAsync(id);
            return deleted ? NoContent() : NotFound("Feedback not found.");
        }
    }
}
