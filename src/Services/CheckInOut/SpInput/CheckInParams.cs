using DataAccess;
using HotelManagement.Services.CheckInOut.Models;

namespace HotelManagement.Services.CheckInOut.SpInput;

public class CheckInParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_check_in";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime CheckInTime { get; set; }
    public CheckInOutStatus Status { get; set; }
    public string? Notes { get; set; }
}
