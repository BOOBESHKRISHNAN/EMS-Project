using EventManagementCL.Data;
using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementCL.Services
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;
        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LocationWithEventsDto>> GetLocationsAsync()
        {
            return await _context.Locations
                .Include(l => l.Events)
                .Select(l => new LocationWithEventsDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    State = l.State,
                    ZipCode = l.ZipCode,
                    VenueFee = l.VenueFee,
                    Events = l.Events.Select(e => new EventDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        LocationId = e.LocationId,
                        Location = l.Name,
                        OrganizerName = e.Organizer.FirstName + " " + e.Organizer.LastName

                    }).ToList() //.Select, EF has already loaded l.Events via .Include(), so it’s safe to use synchronous .ToList() here.
                })
                .ToListAsync();
        }

        public async Task<LocationWithEventsDto?> GetLocationByIdAsync(int id)
        {
            var location = await _context.Locations
                .Include(l => l.Events)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (location == null) return null;
            return new LocationWithEventsDto
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                City = location.City,
                State = location.State,
                ZipCode = location.ZipCode,
                VenueFee = location.VenueFee,
                Events = location.Events.Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    LocationId = e.LocationId,
                    Location = location.Name,
                    OrganizerName = e.Organizer.FirstName + " " + e.Organizer.LastName

               }).ToList()
            };
        }

        public async Task<Location> CreateLocationAsync(LocationCreateDto dto)
        {
            var location = new Location
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                VenueFee = dto.VenueFee,
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location?> UpdateLocationAsync(int id, LocationUpdateDto dto)
        {
            if (id != dto.Id) return null;
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return null;
            location.Name = dto.Name;
            location.Address = dto.Address;
            location.City = dto.City;
            location.State = dto.State;
            location.ZipCode = dto.ZipCode;
            location.VenueFee = dto.VenueFee;

            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
