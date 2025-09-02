using DataAccess;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class CreateTaskParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }

    public string StoredProcedureName => "sp_CreateHousekeepingTask";
}
