using DataAccess;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class GetTasksByRoomIdParams : IStoredProcedureParams
{
    public Guid RoomId { get; set; }

    public string StoredProcedureName => "sp_GetHousekeepingTasksByRoomId";
}
