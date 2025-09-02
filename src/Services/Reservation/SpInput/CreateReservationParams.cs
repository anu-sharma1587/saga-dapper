using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class CreateReservationParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_create_reservation";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfRooms { get; set; }
    public int NumberOfGuests { get; set; }
    public string? SpecialRequests { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
}
