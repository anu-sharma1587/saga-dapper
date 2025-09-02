using HotelManagement.Services.Maintenance.DTOs;
using HotelManagement.Services.Maintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Maintenance.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _service;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(IMaintenanceService service, ILogger<MaintenanceController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<MaintenanceRequestResponse>> CreateRequest([FromBody] CreateRequestDto request)
    {
        var result = await _service.CreateRequestAsync(request);
        return CreatedAtAction(nameof(GetRequest), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaintenanceRequestResponse>> GetRequest(Guid id)
    {
        var result = await _service.GetRequestByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<IEnumerable<MaintenanceRequestResponse>>> GetRequestsByRoom(Guid roomId)
    {
        var result = await _service.GetRequestsByRoomIdAsync(roomId);
        return Ok(result);
    }

    [HttpPost("complete")]
    public async Task<ActionResult<MaintenanceRequestResponse>> CompleteRequest([FromBody] CompleteRequestDto request)
    {
        var result = await _service.CompleteRequestAsync(request);
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{id}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid id)
    {
        var result = await _service.CompensateCancelRequestAsync(id);
        return result ? Ok() : NotFound();
    }
}
