using EventManagementCL.Data;
using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementCL.Services
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _context;

        public FeedbackRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Event)
                .Select(f => new FeedbackResponseDTO
                {
                    FeedbackId = f.FeedbackId,
                    EventId = f.EventId,
                    TicketID = f.TicketID,
                    UserId = f.UserId,
                    Rating = f.Rating,
                    Comments = f.Comments,
                    SubmittedTimestamp = f.SubmittedTimestamp,
                    EventName = f.Event.Title
                })
                .ToListAsync();
        }

        public async Task<FeedbackResponseDTO?> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Event)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null || feedback.Event == null) return null;

            return new FeedbackResponseDTO
            {
                FeedbackId = feedback.FeedbackId,
                EventId = feedback.EventId,
                TicketID = feedback.TicketID,
                UserId = feedback.UserId,
                Rating = feedback.Rating,
                Comments = feedback.Comments,
                SubmittedTimestamp = feedback.SubmittedTimestamp,
                EventName = feedback.Event.Title
            };
        }

        public async Task<IEnumerable<FeedbackResponseDTO>> GetUserFeedbacksAsync(int userId)
        {
            return await _context.Feedbacks
                .Include(f => f.Event)
                .Where(f => f.UserId == userId)
                .Select(f => new FeedbackResponseDTO
                {
                    FeedbackId = f.FeedbackId,
                    EventId = f.EventId,
                    TicketID = f.TicketID,
                    UserId = f.UserId,
                    Rating = f.Rating,
                    Comments = f.Comments,
                    SubmittedTimestamp = f.SubmittedTimestamp,
                    EventName = f.Event.Title
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FeedbackSummaryDTO>> GetEventRatingsSummaryAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Event)
                .GroupBy(f => new { f.EventId, f.Event.Title })
                .Select(g => new FeedbackSummaryDTO
                {
                    EventId = g.Key.EventId,
                    EventName = g.Key.Title,
                    AverageRating = g.Average(f => f.Rating),
                    TotalFeedbacks = g.Count()
                })
                .ToListAsync();
        }

        public async Task<FeedbackResponseDTO?> CreateFeedbackAsync(FeedbackCreateDTO dto)
        {
            var validTicket = await _context.Tickets.AnyAsync(t =>
                t.TicketID == dto.TicketID &&
                t.UserID == dto.UserId &&
                t.EventID == dto.EventId &&
                t.Status == TicketStatus.Confirmed);

            if (!validTicket) return null;

            var feedback = new Feedback
            {
                EventId = dto.EventId,
                TicketID = dto.TicketID,
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comments = dto.Comments,
                SubmittedTimestamp = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var created = await _context.Feedbacks
                .Include(f => f.Event)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedback.FeedbackId);

            if (created == null || created.Event == null) return null;

            return new FeedbackResponseDTO
            {
                FeedbackId = created.FeedbackId,
                EventId = created.EventId,
                TicketID = created.TicketID,
                UserId = created.UserId,
                Rating = created.Rating,
                Comments = created.Comments,
                SubmittedTimestamp = created.SubmittedTimestamp,
                EventName = created.Event.Title
            };
        }

        public async Task<bool> UpdateFeedbackAsync(int id, FeedbackCreateDTO dto)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == id && f.UserId == dto.UserId);
            if (feedback == null) return false;

            feedback.Rating = dto.Rating;
            feedback.Comments = dto.Comments;
            feedback.SubmittedTimestamp = DateTime.UtcNow;

            _context.Feedbacks.Update(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
