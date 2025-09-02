using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class GetReservationsByGuestIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_reservations_by_guest_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid GuestId { get; set; }
}
