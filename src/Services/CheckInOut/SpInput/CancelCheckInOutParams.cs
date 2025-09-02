using DataAccess;
using HotelManagement.Services.CheckInOut.Models;

namespace HotelManagement.Services.CheckInOut.SpInput;

public class CancelCheckInOutParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_cancel_check_in_out";
    public object? p_refcur_1 { get; set; }
    
    public Guid ReservationId { get; set; }
    public CheckInOutStatus Status { get; set; }
}
