namespace HotelManagement.Services.Orchestrator.DTOs;

public record StartReservationSagaRequest(
    Guid GuestId,
    Guid HotelId,
    Guid RoomTypeId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfRooms,
    int NumberOfGuests,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string PaymentProvider
);

public record SagaResult
{
    public Guid SagaId { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
