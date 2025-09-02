using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Services.Guest.Services;
using HotelManagement.Services.Guest.DTOs;
using HotelManagement.BuildingBlocks.Common.Exceptions;

namespace HotelManagement.Services.Guest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;
    private readonly ILogger<GuestsController> _logger;

    public GuestsController(IGuestService guestService, ILogger<GuestsController> logger)
    {
        _guestService = guestService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GuestProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGuestProfile([FromBody] CreateGuestProfileRequest request)
    {
        try
        {
            var profile = await _guestService.CreateGuestProfileAsync(request);
            return CreatedAtAction(nameof(GetGuestProfile), new { id = profile.Id }, profile);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GuestProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGuestProfile(Guid id)
    {
        try
        {
            var profile = await _guestService.GetGuestProfileByIdAsync(id);
            return Ok(profile);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(GuestProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGuestProfileByUserId(Guid userId)
    {
        try
        {
            var profile = await _guestService.GetGuestProfileByUserIdAsync(userId);
            return Ok(profile);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GuestProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGuestProfile(Guid id, [FromBody] UpdateGuestProfileRequest request)
    {
        try
        {
            var profile = await _guestService.UpdateGuestProfileAsync(id, request);
            return Ok(profile);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/preferences")]
    [ProducesResponseType(typeof(GuestProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPreference(Guid id, [FromBody] GuestPreferenceRequest request)
    {
        try
        {
            var profile = await _guestService.AddPreferenceAsync(id, request);
            return Ok(profile);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{guestId}/preferences/{preferenceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePreference(Guid guestId, Guid preferenceId)
    {
        try
        {
            var result = await _guestService.RemovePreferenceAsync(guestId, preferenceId);
            return result ? Ok() : NotFound();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/loyalty-points")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(GuestProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLoyaltyPoints(Guid id, [FromBody] UpdateLoyaltyPointsRequest request)
    {
        try
        {
            var profile = await _guestService.UpdateLoyaltyPointsAsync(id, request);
            return Ok(profile);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(List<GuestProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchGuests([FromQuery] string searchTerm)
    {
        var guests = await _guestService.SearchGuestsAsync(searchTerm);
        return Ok(guests);
    }

    [HttpGet("loyalty/{tier}")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(List<GuestProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGuestsByLoyaltyTier(LoyaltyTier tier)
    {
        var guests = await _guestService.GetGuestsByLoyaltyTierAsync(tier);
        return Ok(guests);
    }
}
