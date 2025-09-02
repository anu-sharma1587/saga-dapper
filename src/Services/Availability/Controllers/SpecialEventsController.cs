using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;
using HotelManagement.Services.Availability.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Availability.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpecialEventsController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<SpecialEventsController> _logger;

    public SpecialEventsController(IAvailabilityService availabilityService, ILogger<SpecialEventsController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<SpecialEvent>>> GetSpecialEvents([FromQuery] Guid hotelId)
    {
        try
        {
            var events = await _availabilityService.GetActiveSpecialEventsAsync(hotelId);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting special events for hotel {HotelId}", hotelId);
            return StatusCode(500, new { message = "An error occurred while retrieving special events" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<SpecialEvent>> CreateSpecialEvent([FromBody] CreateSpecialEventRequest request)
    {
        try
        {
            var specialEvent = new SpecialEvent
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HotelId = request.HotelId,
                ImpactPercentage = request.ImpactPercentage,
                ExpectedDemandIncrease = request.ExpectedDemandIncrease
            };

            var createdEvent = await _availabilityService.CreateSpecialEventAsync(specialEvent);
            return CreatedAtAction(nameof(GetSpecialEvents), new { hotelId = specialEvent.HotelId }, createdEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating special event for hotel {HotelId}", request.HotelId);
            return StatusCode(500, new { message = "An error occurred while creating special event" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SpecialEvent>> UpdateSpecialEvent(Guid id, [FromBody] UpdateSpecialEventRequest request)
    {
        try
        {
            var specialEvent = new SpecialEvent
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HotelId = request.HotelId,
                ImpactPercentage = request.ImpactPercentage,
                ExpectedDemandIncrease = request.ExpectedDemandIncrease,
                IsActive = request.IsActive
            };

            var updatedEvent = await _availabilityService.UpdateSpecialEventAsync(specialEvent);
            return Ok(updatedEvent);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Special event with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating special event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while updating special event" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSpecialEvent(Guid id)
    {
        try
        {
            var success = await _availabilityService.DeleteSpecialEventAsync(id);
            if (success)
            {
                return NoContent();
            }

            return NotFound($"Special event with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting special event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting special event" });
        }
    }
}
