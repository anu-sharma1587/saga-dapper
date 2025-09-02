using HotelManagement.Services.Loyalty.DTOs;
using HotelManagement.Services.Loyalty.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Loyalty.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoyaltyController : ControllerBase
{
    private readonly ILoyaltyService _service;
    private readonly ILogger<LoyaltyController> _logger;

    public LoyaltyController(ILoyaltyService service, ILogger<LoyaltyController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("add-points")]
    public async Task<ActionResult<LoyaltyAccountResponse>> AddPoints([FromBody] AddPointsRequest request)
    {
        var result = await _service.AddPointsAsync(request);
        return Ok(result);
    }

    [HttpPost("redeem-points")]
    public async Task<ActionResult<LoyaltyAccountResponse>> RedeemPoints([FromBody] RedeemPointsRequest request)
    {
        var result = await _service.RedeemPointsAsync(request);
        return Ok(result);
    }

    [HttpGet("guest/{guestId}")]
    public async Task<ActionResult<LoyaltyAccountResponse>> GetAccount(Guid guestId)
    {
        var result = await _service.GetAccountByGuestIdAsync(guestId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{guestId}/compensate-revert-points")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateRevertPoints(Guid guestId, [FromQuery] int points, [FromQuery] string reason)
    {
        var result = await _service.CompensateRevertPointsAsync(guestId, points, reason);
        return result ? Ok() : NotFound();
    }
}
