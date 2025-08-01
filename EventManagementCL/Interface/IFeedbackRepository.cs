using EventManagementCL.DTO;


namespace EventManagementCL.Interface
{
    public interface IFeedbackRepository
    {
        
            Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbacksAsync();
            Task<FeedbackResponseDTO?> GetFeedbackByIdAsync(int id);
            Task<IEnumerable<FeedbackResponseDTO>> GetUserFeedbacksAsync(int userId);
            Task<IEnumerable<FeedbackSummaryDTO>> GetEventRatingsSummaryAsync();

            // Updated return type to send back full feedback details
            Task<FeedbackResponseDTO?> CreateFeedbackAsync(FeedbackCreateDTO dto);

            Task<bool> UpdateFeedbackAsync(int id, FeedbackCreateDTO dto);
            Task<bool> DeleteFeedbackAsync(int id);
        }

    }

