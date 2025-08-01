using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]

    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _repository;
        public LocationController(ILocationRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationWithEventsDto>>> GetLocations()
        {
            var locations = await _repository.GetLocationsAsync();
            return Ok(locations);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<LocationWithEventsDto>> GetLocation(int id)
        {
            var location = await _repository.GetLocationByIdAsync(id);
            if (location == null) return NotFound();
            return Ok(location);
        }


        [HttpPost]
        public async Task<ActionResult<Location>> CreateLocation(LocationCreateDto dto)
        {
            var location = await _repository.CreateLocationAsync(dto);
            return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, LocationUpdateDto dto)
        {
            var updated = await _repository.UpdateLocationAsync(id, dto);
            if (updated == null) return BadRequest();
            return Ok(updated);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)

        {
            var deleted = await _repository.DeleteLocationAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
