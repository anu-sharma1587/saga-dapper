using HotelManagement.Services.CheckInOut.DTOs;
using HotelManagement.Services.CheckInOut.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.CheckInOut.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInOutController : ControllerBase
{
    private readonly ICheckInOutService _service;
    private readonly ILogger<CheckInOutController> _logger;

    public CheckInOutController(ICheckInOutService service, ILogger<CheckInOutController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<CheckInOutResponse>> CheckIn([FromBody] CheckInRequest request)
    {
        var result = await _service.CheckInAsync(request);
        return Ok(result);
    }

    [HttpPost("check-out")]
    public async Task<ActionResult<CheckInOutResponse>> CheckOut([FromBody] CheckOutRequest request)
    {
        var result = await _service.CheckOutAsync(request);
        return Ok(result);
    }

    [HttpGet("reservation/{reservationId}")]
    public async Task<ActionResult<CheckInOutResponse>> GetByReservation(Guid reservationId)
    {
        var result = await _service.GetByReservationIdAsync(reservationId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{reservationId}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid reservationId)
    {
        var result = await _service.CompensateCancelAsync(reservationId);
        return result ? Ok() : NotFound();
    }
}
