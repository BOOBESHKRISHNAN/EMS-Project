using EventManagementCL.Data;
using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using Microsoft.EntityFrameworkCore;


namespace EventManagementCL.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventResponseDTO>> GetEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Location) //Location from DEvent model
                .Select(e => new EventResponseDTO
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate.ToString("dd-MM-yyyy HH:mm:ss"),
                    EndDate = e.EndDate.ToString("dd-MM-yyyy HH:mm:ss"),
                    LocationId = e.LocationId,
                    OrganizerName = e.Organizer.FirstName + " " + e.Organizer.LastName,
                    Location = new EventWithLocationsDTO
                    {
                        Name = e.Location.Name //Name prop of that dto to the name of location associated with current event
                    }
                })
                .ToListAsync();
        }
        public async Task<bool> UpdateTicketPriceAsync(int eventId, decimal newPrice)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return false;

            ev.TicketPrice = newPrice;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<EventResponseDTO?> GetEventByIdAsync(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);

            return ev == null ? null : new EventResponseDTO
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                StartDate = ev.StartDate.ToString("dd-MM-yyyy HH:mm:ss"),
                EndDate = ev.EndDate.ToString("dd-MM-yyyy HH:mm:ss"),
                LocationId = ev.LocationId,
                OrganizerName = ev.Organizer.FirstName + " " + ev.Organizer.LastName,
                Location = new EventWithLocationsDTO
                {
                    Name = ev.Location.Name
                }
            };
        }

        public async Task<EventResponseDTO?> CreateEventAsync(EventCreateDTO dto, int organizerId)
        {
            var overlapping = await IsEventOverlappingAsync(dto.LocationId, dto.StartDate, dto.EndDate);
            if (overlapping) return null;


            var organizer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == organizerId && u.Role == UserRole.Organizer);

            if (organizer == null)
            {
                return null; // Or return a custom error message if you're using a tuple response
            }

            var location = await _context.Locations.FindAsync(dto.LocationId);
            if (location == null) return null;

            var numDays = (dto.EndDate - dto.StartDate).Days + 1;
            var totalVenueFee = location.VenueFee * numDays;

            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TicketPrice = dto.TicketPrice,
                LocationId = dto.LocationId,
                OrganizerId = organizerId,
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(ev.Id);
        }

        public async Task<EventResponseDTO?> UpdateEventAsync(int id, EventCreateDTO dto)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null) return null;

            var overlapping = await IsEventOverlappingAsync(dto.LocationId, dto.StartDate, dto.EndDate, id);
            if (overlapping) return null;

            existingEvent.Title = dto.Title;
            existingEvent.Description = dto.Description;
            existingEvent.StartDate = dto.StartDate;
            existingEvent.EndDate = dto.EndDate;
            existingEvent.LocationId = dto.LocationId;
            existingEvent.OrganizerId = id;

            await _context.SaveChangesAsync();
            return await GetEventByIdAsync(existingEvent.Id);
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return false;

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEventOverlappingAsync(int locationId, DateTime startDate, DateTime endDate, int? excludeEventId = null) // excludeEventId -->while updating it won't check for existing event
        {
            return await _context.Events
                .Where(e => e.LocationId == locationId &&
                    (excludeEventId == null || e.Id != excludeEventId) &&  //If you're creating a new event, this part is ignored. → If you're updating, it excludes the current event from the check.
                    (
                        (startDate >= e.StartDate && startDate < e.EndDate) || // Starts during another event
                        (endDate > e.StartDate && endDate <= e.EndDate) ||  // ends during another event
                        (startDate <= e.StartDate && endDate >= e.EndDate) // totally overlap another event
                    ))
                .AnyAsync(); //Does this query return at least one item?
        }

        public async Task<User?> GetOrganizerByEmailAsync(string? organizerEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == organizerEmail && u.Role == UserRole.Organizer);
        }
    }
}
