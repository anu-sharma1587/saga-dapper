using HotelManagement.Services.Search.DTOs;
using HotelManagement.Services.Search.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Search.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _service;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService service, ILogger<SearchController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<SearchQueryResponse>> CreateSearch([FromBody] CreateSearchRequest request)
    {
        var result = await _service.CreateSearchAsync(request);
        return CreatedAtAction(nameof(GetSearch), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SearchQueryResponse>> GetSearch(Guid id)
    {
        var result = await _service.GetSearchByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<SearchQueryResponse>>> GetSearchesByType(string type)
    {
        var result = await _service.GetSearchesByTypeAsync(type);
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{id}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid id)
    {
        var result = await _service.CompensateCancelSearchAsync(id);
        return result ? Ok() : NotFound();
    }
}
