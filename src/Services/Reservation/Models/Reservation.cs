using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.Reservation.Models;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfRooms { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Payment related
    public Guid? PaymentId { get; set; }
    public bool IsPaid { get; set; }
    public decimal DepositAmount { get; set; }
}

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    CheckedIn,
    CheckedOut,
    NoShow
}
