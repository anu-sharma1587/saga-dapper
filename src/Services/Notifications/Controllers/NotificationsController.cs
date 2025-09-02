using HotelManagement.Services.Notifications.DTOs;
using HotelManagement.Services.Notifications.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Notifications.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService service, ILogger<NotificationsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<NotificationResponse>> SendNotification([FromBody] SendNotificationRequest request)
    {
        var result = await _service.SendNotificationAsync(request);
        return CreatedAtAction(nameof(GetNotification), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationResponse>> GetNotification(Guid id)
    {
        var result = await _service.GetNotificationByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("recipient/{recipientId}")]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetNotificationsByRecipient(Guid recipientId)
    {
        var result = await _service.GetNotificationsByRecipientAsync(recipientId);
        return Ok(result);
    }

    // Compensation endpoint for saga orchestration
    [HttpPost("{id}/compensate-cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateCancel(Guid id)
    {
        var result = await _service.CompensateCancelNotificationAsync(id);
        return result ? Ok() : NotFound();
    }
}
