using DataAccess.Dapper;

namespace HotelManagement.Services.CheckInOut.SpInput;

public class GetCheckInOutByReservationIdParams : IStoredProcedureParams
{
    public Guid ReservationId { get; set; }
}
