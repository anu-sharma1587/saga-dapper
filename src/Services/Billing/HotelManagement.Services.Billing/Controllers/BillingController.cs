using HotelManagement.Services.Billing.DTOs;
using HotelManagement.Services.Billing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Billing.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;
    private readonly ILogger<BillingController> _logger;

    public BillingController(IBillingService billingService, ILogger<BillingController> logger)
    {
        _billingService = billingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var invoice = await _billingService.CreateInvoiceAsync(request);
        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponse>> GetInvoice(Guid id)
    {
        var invoice = await _billingService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpGet("guest/{guestId}")]
    public async Task<ActionResult<IEnumerable<InvoiceResponse>>> GetInvoicesByGuest(Guid guestId)
    {
        var invoices = await _billingService.GetInvoicesByGuestIdAsync(guestId);
        return Ok(invoices);
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> MarkInvoicePaid(Guid id)
    {
        var result = await _billingService.MarkInvoicePaidAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{id}/compensate-void")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateVoidInvoice(Guid id)
    {
        // Used by orchestrator for compensation
        var invoice = await _billingService.GetInvoiceByIdAsync(id);
        if (invoice == null)
            return NotFound();
        var result = await _billingService.MarkInvoicePaidAsync(id); // Mark as paid or void for compensation
        return result ? Ok() : BadRequest();
    }
}
