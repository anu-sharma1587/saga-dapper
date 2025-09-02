using DataAccess;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class GetTaskByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }

    public string StoredProcedureName => "sp_GetHousekeepingTaskById";
}
