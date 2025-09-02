using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class MarkAsNoShowParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_mark_no_show";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public DateTime ModifiedAt { get; set; }
}
