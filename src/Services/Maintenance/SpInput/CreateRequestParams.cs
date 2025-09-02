using DataAccess;

namespace HotelManagement.Services.Maintenance.SpInput;

public class CreateRequestParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string Description { get; set; } = null!;
    public Guid? AssignedStaffId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }

    public string StoredProcedureName => "sp_CreateMaintenanceRequest";
}
