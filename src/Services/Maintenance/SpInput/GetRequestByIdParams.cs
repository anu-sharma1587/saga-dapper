using DataAccess;

namespace HotelManagement.Services.Maintenance.SpInput;

public class GetRequestByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }

    public string StoredProcedureName => "sp_GetMaintenanceRequestById";
}
