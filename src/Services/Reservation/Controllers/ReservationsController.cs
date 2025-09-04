using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Services.Reservation.Services;
using HotelManagement.Services.Reservation.DTOs;

namespace HotelManagement.Services.Reservation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<ReservationsController> _logger;

    public ReservationsController(
        IReservationService reservationService,
        ILogger<ReservationsController> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReservation(Guid id)
    {
        try
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            return Ok(reservation);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("guest/{guestId}")]
    [ProducesResponseType(typeof(IEnumerable<ReservationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationsByGuest(Guid guestId)
    {
        var reservations = await _reservationService.GetReservationsByGuestIdAsync(guestId);
        return Ok(reservations);
    }

    [HttpGet("hotel/{hotelId}")]
    [ProducesResponseType(typeof(IEnumerable<ReservationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationsByHotel(
        Guid hotelId, 
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var reservations = await _reservationService.GetReservationsByHotelIdAsync(hotelId, fromDate, toDate);
        return Ok(reservations);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateReservationStatus(
        Guid id,
        [FromBody] UpdateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.UpdateReservationStatusAsync(id, request);
            return Ok(reservation);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelReservation(Guid id, [FromBody] CancelReservationRequest request)
    {
        try
        {
            var result = await _reservationService.CancelReservationAsync(id, request.Reason);
            if (!result)
            {
                return NotFound(new { message = $"Reservation with ID {id} not found." });
            }
            return Ok(new { message = "Reservation cancelled successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling reservation with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while cancelling the reservation." });
        }
    }

    [HttpPost("{id}/checkin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckIn(Guid id)
    {
        try
        {
            var result = await _reservationService.CheckInAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Reservation with ID {id} not found." });
            }
            return Ok(new { message = "Check-in successful." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in reservation with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred during check-in." });
        }
    }

    [HttpPost("{id}/checkout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckOut(Guid id)
    {
        try
        {
            var result = await _reservationService.CheckOutAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Reservation with ID {id} not found." });
            }
            return Ok(new { message = "Check-out successful." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out reservation with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred during check-out." });
        }
    }

    [HttpPost("{id}/noshow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsNoShow(Guid id)
    {
        try
        {
            var result = await _reservationService.MarkAsNoShowAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Reservation with ID {id} not found." });
            }
            return Ok(new { message = "Reservation marked as no-show." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking reservation with ID {Id} as no-show", id);
            return StatusCode(500, new { message = "An error occurred while marking the reservation as no-show." });
        }
    }

    [HttpPatch("{id}/payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaymentStatus(
        Guid id,
        [FromBody] UpdatePaymentStatusRequest request)
    {
        try
        {
            var result = await _reservationService.UpdatePaymentStatusAsync(id, request.IsPaid, request.DepositAmount);
            if (!result)
            {
                return NotFound(new { message = $"Reservation with ID {id} not found." });
            }
            return Ok(new { message = "Payment status updated successfully." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Reservation with ID {id} not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for reservation with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the payment status." });
        }
    }
}

public record CancelReservationRequest(string Reason);
public record UpdatePaymentStatusRequest(bool IsPaid, decimal? DepositAmount = null);
