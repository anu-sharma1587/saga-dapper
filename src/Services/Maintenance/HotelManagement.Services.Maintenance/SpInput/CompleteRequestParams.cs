using DataAccess;

namespace HotelManagement.Services.Maintenance.SpInput;

public class CompleteRequestParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Status { get; set; } = "Completed";
    public string? Notes { get; set; }

    public string StoredProcedureName => "sp_CompleteMaintenanceRequest";
}
