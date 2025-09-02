using HotelManagement.Services.Housekeeping.DTOs;

namespace HotelManagement.Services.Housekeeping.Services;

public interface IHousekeepingService
{
    Task<HousekeepingTaskResponse> CreateTaskAsync(CreateTaskRequest request);
    Task<HousekeepingTaskResponse?> GetTaskByIdAsync(Guid id);
    Task<IEnumerable<HousekeepingTaskResponse>> GetTasksByRoomIdAsync(Guid roomId);
    Task<HousekeepingTaskResponse> CompleteTaskAsync(CompleteTaskRequest request);
    Task<bool> CompensateCancelTaskAsync(Guid id);
}
