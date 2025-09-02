using HotelManagement.Services.Housekeeping.DTOs;
using HotelManagement.Services.Housekeeping.Models;
using HotelManagement.Services.Housekeeping.SpInput;
using DataAccess;

namespace HotelManagement.Services.Housekeeping.Services;

public class HousekeepingService : IHousekeepingService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<HousekeepingService> _logger;

    public HousekeepingService(IDataRepository dataRepository, ILogger<HousekeepingService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<HousekeepingTaskResponse> CreateTaskAsync(CreateTaskRequest request)
    {
        try
        {
            var parameters = new CreateTaskParams
            {
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                AssignedStaffId = request.AssignedStaffId,
                ScheduledAt = request.ScheduledAt,
                Status = "Pending",
                Notes = request.Notes
            };

            var result = await _dataRepository.ExecuteStoredProcedureAsync<HousekeepingTask, CreateTaskParams>(parameters);
            
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating housekeeping task");
            throw;
        }
    }

    public async Task<HousekeepingTaskResponse?> GetTaskByIdAsync(Guid id)
    {
        try
        {
            var parameters = new GetTaskByIdParams { Id = id };
            var result = await _dataRepository.QueryFirstOrDefaultStoredProcedureAsync<HousekeepingTask, GetTaskByIdParams>(parameters);
            
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting housekeeping task by id {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<HousekeepingTaskResponse>> GetTasksByRoomIdAsync(Guid roomId)
    {
        try
        {
            var parameters = new GetTasksByRoomIdParams { RoomId = roomId };
            var results = await _dataRepository.QueryStoredProcedureAsync<HousekeepingTask, GetTasksByRoomIdParams>(parameters);
            
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting housekeeping tasks by room id {RoomId}", roomId);
            return Enumerable.Empty<HousekeepingTaskResponse>();
        }
    }

    public async Task<HousekeepingTaskResponse> CompleteTaskAsync(CompleteTaskRequest request)
    {
        try
        {
            var parameters = new CompleteTaskParams 
            { 
                Id = request.TaskId,
                CompletedAt = DateTime.UtcNow,
                Status = "Completed",
                Notes = request.Notes
            };
            
            var result = await _dataRepository.ExecuteStoredProcedureAsync<HousekeepingTask, CompleteTaskParams>(parameters);
            
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing housekeeping task {Id}", request.TaskId);
            throw;
        }
    }

    public async Task<bool> CompensateCancelTaskAsync(Guid id)
    {
        try
        {
            var parameters = new CompensateCancelTaskParams 
            { 
                Id = id,
                Status = "Cancelled"
            };
            
            var result = await _dataRepository.ExecuteStoredProcedureAsync<CompensateCancelTaskParams>(parameters);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling housekeeping task {Id}", id);
            return false;
        }
    }

    private static HousekeepingTaskResponse MapToResponse(HousekeepingTask task) => new()
    {
        Id = task.Id,
        RoomId = task.RoomId,
        AssignedStaffId = task.AssignedStaffId,
        ScheduledAt = task.ScheduledAt,
        CompletedAt = task.CompletedAt,
        Status = task.Status.ToString(),
        Notes = task.Notes
    };
}
