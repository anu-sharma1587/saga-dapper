using DataAccess;

namespace HotelManagement.Services.Maintenance.SpInput;

public class CompensateCancelRequestParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Cancelled";

    public string StoredProcedureName => "sp_CancelMaintenanceRequest";
}
