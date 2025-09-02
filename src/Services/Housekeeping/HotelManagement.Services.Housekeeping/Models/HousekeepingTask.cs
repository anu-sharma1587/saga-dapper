namespace HotelManagement.Services.Housekeeping.Models;

public class HousekeepingTask
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public HousekeepingStatus Status { get; set; }
    public string? Notes { get; set; }
}

public enum HousekeepingStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}
