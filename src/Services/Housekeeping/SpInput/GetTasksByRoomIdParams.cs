using DataAccess.Dapper;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class GetTasksByRoomIdParams : IStoredProcedureParams
{
    public Guid RoomId { get; set; }
}
