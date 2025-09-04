using DataAccess.Dapper;

namespace HotelManagement.Services.Maintenance.SpInput;

public class CompensateCancelRequestParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Cancelled";
}
