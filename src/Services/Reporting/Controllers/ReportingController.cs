using HotelManagement.Services.Reporting.DTOs;
using HotelManagement.Services.Reporting.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Reporting.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _service;
    private readonly ILogger<ReportingController> _logger;

    public ReportingController(IReportingService service, ILogger<ReportingController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ReportJobResponse>> CreateReport([FromBody] CreateReportRequest request)
    {
        var result = await _service.CreateReportAsync(request);
        return CreatedAtAction(nameof(GetReport), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReportJobResponse>> GetReport(Guid id)
    {
        var result = await _service.GetReportByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<ReportJobResponse>>> GetReportsByType(string type)
    {
        var result = await _service.GetReportsByTypeAsync(type);
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{id}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid id)
    {
        var result = await _service.CompensateCancelReportAsync(id);
        return result ? Ok() : NotFound();
    }
}
