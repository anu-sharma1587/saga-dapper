using HotelManagement.Services.Payment.DTOs;
using HotelManagement.Services.Payment.Models;
using HotelManagement.Services.Payment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Payment.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResult>> ProcessPayment([FromBody] PaymentRequest request)
    {
        var result = await _paymentService.ProcessPaymentAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("refund")]
    public async Task<ActionResult<RefundResult>> RefundPayment([FromBody] RefundRequest request)
    {
        var result = await _paymentService.RefundPaymentAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Models.Payment>> GetPayment(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [HttpGet("reservation/{reservationId}")]
    public async Task<ActionResult<IEnumerable<Models.Payment>>> GetPaymentsByReservation(Guid reservationId)
    {
        var payments = await _paymentService.GetPaymentsByReservationIdAsync(reservationId);
        return Ok(payments);
    }

    [HttpPost("{id}/compensate-refund")]
    [AllowAnonymous]
    public async Task<IActionResult> CompensateRefund(Guid id, [FromQuery] decimal? amount = null)
    {
        // Used by orchestrator for compensation
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();
        var refundRequest = new RefundRequest(id, amount ?? payment.Amount, "Saga compensation");
        var result = await _paymentService.RefundPaymentAsync(refundRequest);
        return result.Success ? Ok() : BadRequest(result.ErrorMessage);
    }
}
