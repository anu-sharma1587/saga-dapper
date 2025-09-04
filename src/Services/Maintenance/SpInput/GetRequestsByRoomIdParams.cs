using DataAccess.Dapper;

namespace HotelManagement.Services.Maintenance.SpInput;

public class GetRequestsByRoomIdParams : IStoredProcedureParams
{
    public Guid RoomId { get; set; }
}
