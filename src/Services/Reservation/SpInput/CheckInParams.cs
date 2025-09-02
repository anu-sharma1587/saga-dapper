using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class CheckInParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_check_in_reservation";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public DateTime ModifiedAt { get; set; }
}
