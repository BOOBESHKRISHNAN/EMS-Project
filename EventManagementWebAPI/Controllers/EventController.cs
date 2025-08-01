        using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using EventManagementCL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]  //serve http api response
[Route("api/[controller]")]  
public class EventController : ControllerBase //base class of mvc without view support
{
    private readonly IEventRepository _repository;
    private readonly NotificationService _notificationService;

    //constructor injection
    public EventController(IEventRepository repository, NotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}, {nameof(UserRole.Admin)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.RegisteredUser)}")]
    public async Task<ActionResult<IEnumerable<EventResponseDTO>>> GetEvents()
    {
        var events = await _repository.GetEventsAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}, {nameof(UserRole.Admin)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.RegisteredUser)}")]
    public async Task<ActionResult<EventResponseDTO>> GetEventById(int id)
    {
        var ev = await _repository.GetEventByIdAsync(id);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreateDTO dto)
    {
        var organizerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (organizerIdClaim == null)
            return Unauthorized("Organizer identity not found.");

        if (!int.TryParse(organizerIdClaim.Value, out int organizerId))
            return Unauthorized("Invalid organizer ID.");

        // Create event using organizer's ID
        var response = await _repository.CreateEventAsync(dto, organizerId);
        if (response == null)
            return Conflict("Event could not be created. It may overlap with another event or the organizer is invalid.");

        // Get organizer's email from token
        var organizerEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        // Lookup organizer user
        var organizer = await _repository.GetOrganizerByEmailAsync(organizerEmail); //Queries the database for the organizer’s full profile using their email.

        // Send notification (no assignment!)
        if (organizer != null)
            await _notificationService.SendEventCreationEmail(organizer, response);

        return CreatedAtAction(nameof(GetEventById), new { id = response.Id }, response);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Organizer,SuperAdmin,Admin")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventCreateDTO dto)
    {
        var overlap = await _repository.IsEventOverlappingAsync(dto.LocationId, dto.StartDate, dto.EndDate, id);
        if (overlap)
            return Conflict("Another event already exists at this location during the selected time.");

        var response = await _repository.UpdateEventAsync(id, dto);
        if (response == null)
            return NotFound($"Event with ID {id} not found.");

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer,SuperAdmin,Admin")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var deleted = await _repository.DeleteEventAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}