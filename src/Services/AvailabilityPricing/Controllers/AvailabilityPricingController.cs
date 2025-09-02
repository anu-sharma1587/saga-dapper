using HotelManagement.Services.AvailabilityPricing.DTOs;
using HotelManagement.Services.AvailabilityPricing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.AvailabilityPricing.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvailabilityPricingController : ControllerBase
{
    private readonly IAvailabilityPricingService _service;
    private readonly ILogger<AvailabilityPricingController> _logger;

    public AvailabilityPricingController(IAvailabilityPricingService service, ILogger<AvailabilityPricingController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AvailabilityPricingResponse>> CreateAvailabilityPricing([FromBody] CreateAvailabilityPricingRequest request)
    {
        var result = await _service.CreateAvailabilityPricingAsync(request);
        return CreatedAtAction(nameof(GetAvailabilityPricing), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AvailabilityPricingResponse>> GetAvailabilityPricing(Guid id)
    {
        var result = await _service.GetAvailabilityPricingByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<AvailabilityPricingResponse>>> GetAvailabilityPricingsByHotel(Guid hotelId)
    {
        var result = await _service.GetAvailabilityPricingsByHotelAsync(hotelId);
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{id}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid id)
    {
        var result = await _service.CompensateCancelAvailabilityPricingAsync(id);
        return result ? Ok() : NotFound();
    }
}
