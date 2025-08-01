using EventManagementCL.DTO;
using EventManagementCL.Models;


namespace EventManagementCL.Interface
{
    public interface IEventRepository
    {
        // Fetch all events with details
        Task<IEnumerable<EventResponseDTO>> GetEventsAsync();

        // Fetch a single event by
        Task<EventResponseDTO?> GetEventByIdAsync(int id);

        // Create a new event
        Task<EventResponseDTO?> CreateEventAsync(EventCreateDTO dto, int organizerId);

        // Update an existing event
        Task<EventResponseDTO?> UpdateEventAsync(int id, EventCreateDTO dto);

        // Delete an event by ID
        Task<bool> DeleteEventAsync(int id);   //<bool> -->true/false

        // 🧠 Check if scheduling conflicts exist for location & time
        Task<bool> IsEventOverlappingAsync(int locationId, DateTime startDate, DateTime endDate, int? excludeEventId = null);
        Task<User?> GetOrganizerByEmailAsync(string? organizerEmail);  //It looks up whether an organizer exists with that email.



    }
}
