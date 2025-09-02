namespace HotelManagement.Services.CheckInOut.DTOs;

public record CheckInRequest(Guid ReservationId, Guid GuestId, string? Notes);
public record CheckOutRequest(Guid ReservationId, Guid GuestId, string? Notes);
public record CheckInOutResponse
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
}
