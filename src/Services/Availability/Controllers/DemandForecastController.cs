using HotelManagement.Services.Availability.Models.Dtos;
using HotelManagement.Services.Availability.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Availability.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DemandForecastController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<DemandForecastController> _logger;

    public DemandForecastController(IAvailabilityService availabilityService, ILogger<DemandForecastController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<DemandForecast>>> GetDemandForecasts(
        [FromQuery] Guid hotelId,
        [FromQuery] Guid roomTypeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate >= endDate)
            {
                return BadRequest("Start date must be before end date");
            }

            var forecasts = await _availabilityService.GetDemandForecastsAsync(hotelId, roomTypeId, startDate, endDate);
            return Ok(forecasts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting demand forecasts for hotel {HotelId}, room type {RoomTypeId}", 
                hotelId, roomTypeId);
            return StatusCode(500, new { message = "An error occurred while retrieving demand forecasts" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> UpdateDemandForecast([FromBody] UpdateDemandForecastRequest request)
    {
        try
        {
            var forecast = new DemandForecast
            {
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                Date = request.Date,
                ExpectedDemand = request.ExpectedDemand,
                SuggestedPriceAdjustment = (double)request.SuggestedPriceAdjustment,
                Factors = request.Factors != null ? System.Text.Json.JsonSerializer.Serialize(request.Factors) : string.Empty,
            };

            await _availabilityService.UpdateDemandForecastAsync(forecast);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating demand forecast for hotel {HotelId}, room type {RoomTypeId}",
                request.HotelId, request.RoomTypeId);
            return StatusCode(500, new { message = "An error occurred while updating demand forecast" });
        }
    }
}
