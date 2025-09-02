namespace HotelManagement.Services.Housekeeping.DTOs;

public record CreateTaskRequest(Guid RoomId, Guid? AssignedStaffId, DateTime ScheduledAt, string? Notes);
public record CompleteTaskRequest(Guid TaskId, string? Notes);
public record HousekeepingTaskResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
}
