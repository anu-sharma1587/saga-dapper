namespace HotelManagement.Services.Maintenance.DTOs;

public record CreateRequestDto(Guid RoomId, string Description, Guid? AssignedStaffId, string? Notes);
public record CompleteRequestDto(Guid RequestId, string? Notes);
public record MaintenanceRequestResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string Description { get; set; } = null!;
    public Guid? AssignedStaffId { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
}
