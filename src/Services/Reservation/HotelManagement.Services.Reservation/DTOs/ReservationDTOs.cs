using System.Text.Json.Serialization;

namespace HotelManagement.Services.Reservation.DTOs;

public record CreateReservationRequest(
    Guid HotelId,
    Guid RoomTypeId,
    Guid GuestId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfRooms,
    int NumberOfGuests,
    string? SpecialRequests
);

public record UpdateReservationRequest(
    ReservationStatus Status,
    string? CancellationReason = null
);

public record ReservationResponse
{
    public Guid Id { get; init; }
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public Guid GuestId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumberOfRooms { get; init; }
    public int NumberOfGuests { get; init; }
    public decimal TotalPrice { get; init; }
    public ReservationStatus Status { get; init; }
    public string? SpecialRequests { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? CancellationReason { get; init; }
    public DateTime? CancelledAt { get; init; }
    public bool IsPaid { get; init; }
    public decimal DepositAmount { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    CheckedIn,
    CheckedOut,
    NoShow
}
