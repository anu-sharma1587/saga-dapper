using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class GetReservationByIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_reservation_by_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
