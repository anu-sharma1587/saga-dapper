using DataAccess.Dapper;

namespace HotelManagement.Services.CheckInOut.SpInput;

public class CancelCheckInOutParams : IStoredProcedureParams
{
    public Guid ReservationId { get; set; }
    public string Status { get; set; } = "Cancelled";
}
