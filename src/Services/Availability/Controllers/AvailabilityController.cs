using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;
using HotelManagement.Services.Availability.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Availability.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(IAvailabilityService availabilityService, ILogger<AvailabilityController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomAvailabilityResponse>>> GetAvailability(
        [FromQuery] Guid hotelId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] List<Guid>? roomTypeIds = null)
    {
        try
        {
            if (checkIn >= checkOut)
            {
                return BadRequest("Check-in date must be before check-out date");
            }

            var availability = await _availabilityService.GetAvailabilityAsync(hotelId, checkIn, checkOut, roomTypeIds);
            return Ok(availability);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting availability for hotel {HotelId}", hotelId);
            return StatusCode(500, new { message = "An error occurred while retrieving availability" });
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult> UpdateAvailability([FromBody] UpdateAvailabilityRequest request)
    {
        try
        {
            var success = await _availabilityService.UpdateAvailabilityAsync(
                request.HotelId,
                request.RoomTypeId,
                request.Date,
                request.AvailableRooms,
                request.BasePrice);

            if (success)
            {
                return Ok();
            }

            return NotFound($"No availability record found for hotel {request.HotelId}, room type {request.RoomTypeId}");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability for hotel {HotelId}, room type {RoomTypeId}",
                request.HotelId, request.RoomTypeId);
            return StatusCode(500, new { message = "An error occurred while updating availability" });
        }
    }

    [Authorize]
    [HttpPut("batch")]
    public async Task<ActionResult<List<bool>>> UpdateAvailabilityBatch([FromBody] BatchUpdateAvailabilityRequest request)
    {
        try
        {
            var results = new List<bool>();
            foreach (var update in request.Updates)
            {
                var success = await _availabilityService.UpdateAvailabilityAsync(
                    update.HotelId,
                    update.RoomTypeId,
                    update.Date,
                    update.AvailableRooms,
                    update.BasePrice);
                results.Add(success);
            }
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability batch");
            return StatusCode(500, new { message = "An error occurred while updating availability batch" });
        }
    }

    [Authorize]
    [HttpPost("analyze")]
    public async Task<ActionResult<PricingAnalysisResponse>> GetPricingAnalysis([FromBody] PriceAnalysisRequest request)
    {
        try
        {
            var analysis = await _availabilityService.GetPricingAnalysisAsync(
                request.HotelId,
                request.RoomTypeId,
                request.Date);

            return Ok(analysis);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing pricing for hotel {HotelId}, room type {RoomTypeId}",
                request.HotelId, request.RoomTypeId);
            return StatusCode(500, new { message = "An error occurred while analyzing pricing" });
        }
    }
}
