namespace HotelManagement.Services.CheckInOut.Models;

public class CheckInOutRecord
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public CheckInOutStatus Status { get; set; }
    public string? Notes { get; set; }
}

public enum CheckInOutStatus
{
    Pending,
    CheckedIn,
    CheckedOut,
    Cancelled
}
