using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;
using HotelManagement.Services.Availability.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Availability.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeasonalPeriodsController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<SeasonalPeriodsController> _logger;

    public SeasonalPeriodsController(IAvailabilityService availabilityService, ILogger<SeasonalPeriodsController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<SeasonalPeriod>>> GetSeasonalPeriods([FromQuery] Guid hotelId)
    {
        try
        {
            var periods = await _availabilityService.GetActiveSeasonalPeriodsAsync(hotelId);
            return Ok(periods);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seasonal periods for hotel {HotelId}", hotelId);
            return StatusCode(500, new { message = "An error occurred while retrieving seasonal periods" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<SeasonalPeriod>> CreateSeasonalPeriod([FromBody] CreateSeasonalPeriodRequest request)
    {
        try
        {
            var period = new SeasonalPeriod
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HotelId = request.HotelId,
                BaseAdjustmentPercentage = request.BaseAdjustmentPercentage
            };

            var createdPeriod = await _availabilityService.CreateSeasonalPeriodAsync(period);
            return CreatedAtAction(nameof(GetSeasonalPeriods), new { hotelId = period.HotelId }, createdPeriod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seasonal period for hotel {HotelId}", request.HotelId);
            return StatusCode(500, new { message = "An error occurred while creating seasonal period" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SeasonalPeriod>> UpdateSeasonalPeriod(Guid id, [FromBody] UpdateSeasonalPeriodRequest request)
    {
        try
        {
            var period = new SeasonalPeriod
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HotelId = request.HotelId,
                BaseAdjustmentPercentage = request.BaseAdjustmentPercentage,
                IsActive = request.IsActive
            };

            var updatedPeriod = await _availabilityService.UpdateSeasonalPeriodAsync(period);
            return Ok(updatedPeriod);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Seasonal period with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating seasonal period {PeriodId}", id);
            return StatusCode(500, new { message = "An error occurred while updating seasonal period" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSeasonalPeriod(Guid id)
    {
        try
        {
            var success = await _availabilityService.DeleteSeasonalPeriodAsync(id);
            if (success)
            {
                return NoContent();
            }

            return NotFound($"Seasonal period with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting seasonal period {PeriodId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting seasonal period" });
        }
    }
}
