using DataAccess.Dapper;
using HotelManagement.Services.CheckInOut.Models;

namespace HotelManagement.Services.CheckInOut.SpInput;

public class CheckOutParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_check_out";
    public object? p_refcur_1 { get; set; }
    
    public Guid ReservationId { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string Status { get; set; } = "CheckedOut";
    public string? Notes { get; set; }
}
