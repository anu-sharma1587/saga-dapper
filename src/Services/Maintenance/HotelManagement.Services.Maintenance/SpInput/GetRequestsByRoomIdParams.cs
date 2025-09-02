using DataAccess;

namespace HotelManagement.Services.Maintenance.SpInput;

public class GetRequestsByRoomIdParams : IStoredProcedureParams
{
    public Guid RoomId { get; set; }

    public string StoredProcedureName => "sp_GetMaintenanceRequestsByRoomId";
}
