using DataAccess;

namespace HotelManagement.Services.CheckInOut.SpInput;

public class GetCheckInOutByReservationIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_check_in_out_by_reservation_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid ReservationId { get; set; }
}
