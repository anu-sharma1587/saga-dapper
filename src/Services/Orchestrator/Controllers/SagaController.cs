using HotelManagement.Services.Orchestrator.DTOs;
using HotelManagement.Services.Orchestrator.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SagaController : ControllerBase
{
    private readonly ISagaOrchestrator _orchestrator;
    private readonly ILogger<SagaController> _logger;

    public SagaController(ISagaOrchestrator orchestrator, ILogger<SagaController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    [HttpPost("reservation")]
    public async Task<ActionResult<SagaResult>> StartReservationSaga([FromBody] StartReservationSagaRequest request)
    {
        var result = await _orchestrator.StartReservationSagaAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
