using HotelManagement.Services.Housekeeping.DTOs;
using HotelManagement.Services.Housekeeping.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Housekeeping.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HousekeepingController : ControllerBase
{
    private readonly IHousekeepingService _service;
    private readonly ILogger<HousekeepingController> _logger;

    public HousekeepingController(IHousekeepingService service, ILogger<HousekeepingController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<HousekeepingTaskResponse>> CreateTask([FromBody] CreateTaskRequest request)
    {
        var result = await _service.CreateTaskAsync(request);
        return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HousekeepingTaskResponse>> GetTask(Guid id)
    {
        var result = await _service.GetTaskByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<IEnumerable<HousekeepingTaskResponse>>> GetTasksByRoom(Guid roomId)
    {
        var result = await _service.GetTasksByRoomIdAsync(roomId);
        return Ok(result);
    }

    [HttpPost("complete")]
    public async Task<ActionResult<HousekeepingTaskResponse>> CompleteTask([FromBody] CompleteTaskRequest request)
    {
        var result = await _service.CompleteTaskAsync(request);
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{id}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid id)
    {
        var result = await _service.CompensateCancelTaskAsync(id);
        return result ? Ok() : NotFound();
    }
}
