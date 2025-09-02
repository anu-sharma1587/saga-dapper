namespace HotelManagement.Services.Maintenance.Models;

public class MaintenanceRequest
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string Description { get; set; } = null!;
    public Guid? AssignedStaffId { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public MaintenanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public enum MaintenanceStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}
