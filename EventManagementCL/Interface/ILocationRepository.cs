using EventManagementCL.DTO;
using EventManagementCL.Models;


namespace EventManagementCL.Interface
{
    public interface ILocationRepository
    {
        Task<IEnumerable<LocationWithEventsDto>> GetLocationsAsync();
        Task<LocationWithEventsDto?> GetLocationByIdAsync(int id);
        Task<Location> CreateLocationAsync(LocationCreateDto dto);
        Task<Location?> UpdateLocationAsync(int id, LocationUpdateDto dto);
        Task<bool> DeleteLocationAsync(int id);
    }
}
