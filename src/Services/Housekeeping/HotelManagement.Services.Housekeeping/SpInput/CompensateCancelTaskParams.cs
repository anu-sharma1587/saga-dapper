using DataAccess;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class CompensateCancelTaskParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Cancelled";

    public string StoredProcedureName => "sp_CancelHousekeepingTask";
}
