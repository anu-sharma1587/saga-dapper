using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;
using HotelManagement.Services.Availability.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Availability.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PricingRulesController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<PricingRulesController> _logger;

    public PricingRulesController(IAvailabilityService availabilityService, ILogger<PricingRulesController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<PricingRule>>> GetPricingRules([FromQuery] Guid hotelId, [FromQuery] Guid? roomTypeId = null)
    {
        try
        {
            var rules = await _availabilityService.GetActivePricingRulesAsync(hotelId, roomTypeId);
            return Ok(rules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pricing rules for hotel {HotelId}", hotelId);
            return StatusCode(500, new { message = "An error occurred while retrieving pricing rules" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<PricingRule>> CreatePricingRule([FromBody] CreatePricingRuleRequest request)
    {
        try
        {
            var rule = new PricingRule
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                DaysOfWeek = request.DaysOfWeek,
                AdjustmentPercentage = request.AdjustmentPercentage,
                Priority = request.Priority
            };

            var createdRule = await _availabilityService.CreatePricingRuleAsync(rule);
            return CreatedAtAction(nameof(GetPricingRules), new { hotelId = rule.HotelId }, createdRule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pricing rule for hotel {HotelId}", request.HotelId);
            return StatusCode(500, new { message = "An error occurred while creating pricing rule" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PricingRule>> UpdatePricingRule(Guid id, [FromBody] UpdatePricingRuleRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in URL does not match ID in request body");
            }

            var rule = new PricingRule
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                DaysOfWeek = request.DaysOfWeek,
                AdjustmentPercentage = request.AdjustmentPercentage,
                Priority = request.Priority
            };

            var updatedRule = await _availabilityService.UpdatePricingRuleAsync(rule);
            return Ok(updatedRule);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Pricing rule with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pricing rule {RuleId}", id);
            return StatusCode(500, new { message = "An error occurred while updating pricing rule" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePricingRule(Guid id)
    {
        try
        {
            var success = await _availabilityService.DeletePricingRuleAsync(id);
            if (success)
            {
                return NoContent();
            }

            return NotFound($"Pricing rule with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pricing rule {RuleId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting pricing rule" });
        }
    }
}
