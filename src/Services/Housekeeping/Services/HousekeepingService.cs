using HotelManagement.Services.Housekeeping.DTOs;
using HotelManagement.Services.Housekeeping.Models;
using HotelManagement.Services.Housekeeping.SpInput;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.Housekeeping.Services;

public class HousekeepingService : IHousekeepingService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<HousekeepingService> _logger;

    public HousekeepingService(IDapperDataRepository dataRepository, IDbConnectionFactory connectionFactory, ILogger<HousekeepingService> logger)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<HousekeepingTaskResponse> CreateTaskAsync(CreateTaskRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            var task = new HousekeepingTask
            {
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                AssignedStaffId = request.AssignedStaffId,
                ScheduledAt = request.ScheduledAt,
                Status = HousekeepingStatus.Pending,
                Notes = request.Notes
            };

            await _dataRepository.AddAsync(task, connection);
            
            return MapToResponse(task);
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
            using var connection = await _connectionFactory.CreateAsync();
            var result = await _dataRepository.FindByIDAsync<HousekeepingTask>(id, connection);
            
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
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetTasksByRoomIdAsync not fully implemented with current interface");
            return Enumerable.Empty<HousekeepingTaskResponse>();
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
            using var connection = await _connectionFactory.CreateAsync();
            
            var task = await _dataRepository.FindByIDAsync<HousekeepingTask>(request.TaskId, connection);
            if (task == null)
            {
                throw new Exception("Housekeeping task not found");
            }
            
            task.CompletedAt = DateTime.UtcNow;
            task.Status = HousekeepingStatus.Completed;
            task.Notes = request.Notes;
            
            await _dataRepository.UpdateAsync(task, task.Id, connection);
            
            return MapToResponse(task);
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
            using var connection = await _connectionFactory.CreateAsync();
            
            var task = await _dataRepository.FindByIDAsync<HousekeepingTask>(id, connection);
            if (task == null)
            {
                return false;
            }
            
            task.Status = HousekeepingStatus.Cancelled;
            var result = await _dataRepository.UpdateAsync(task, task.Id, connection);
            
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
