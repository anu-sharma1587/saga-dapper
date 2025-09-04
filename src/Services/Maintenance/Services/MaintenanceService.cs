using HotelManagement.Services.Maintenance.DTOs;
using HotelManagement.Services.Maintenance.Models;
using HotelManagement.Services.Maintenance.SpInput;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.Maintenance.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<MaintenanceService> _logger;

    public MaintenanceService(IDapperDataRepository dataRepository, IDbConnectionFactory connectionFactory, ILogger<MaintenanceService> logger)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<MaintenanceRequestResponse> CreateRequestAsync(CreateRequestDto request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            var maintenanceRequest = new MaintenanceRequest
            {
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                Description = request.Description,
                AssignedStaffId = request.AssignedStaffId,
                RequestedAt = DateTime.UtcNow,
                Status = MaintenanceStatus.Pending,
                Notes = request.Notes
            };

            await _dataRepository.AddAsync(maintenanceRequest, connection);
            
            return MapToResponse(maintenanceRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating maintenance request");
            throw;
        }
    }

    public async Task<MaintenanceRequestResponse?> GetRequestByIdAsync(Guid id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            var result = await _dataRepository.FindByIDAsync<MaintenanceRequest>(id, connection);
            
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting maintenance request by id {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<MaintenanceRequestResponse>> GetRequestsByRoomIdAsync(Guid roomId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetRequestsByRoomIdAsync not fully implemented with current interface");
            return Enumerable.Empty<MaintenanceRequestResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting maintenance requests by room id {RoomId}", roomId);
            return Enumerable.Empty<MaintenanceRequestResponse>();
        }
    }

    public async Task<MaintenanceRequestResponse> CompleteRequestAsync(CompleteRequestDto request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            var maintenanceRequest = await _dataRepository.FindByIDAsync<MaintenanceRequest>(request.RequestId, connection);
            if (maintenanceRequest == null)
            {
                throw new Exception("Maintenance request not found");
            }
            
            maintenanceRequest.CompletedAt = DateTime.UtcNow;
            maintenanceRequest.Status = MaintenanceStatus.Completed;
            maintenanceRequest.Notes = request.Notes;
            
            await _dataRepository.UpdateAsync(maintenanceRequest, maintenanceRequest.Id, connection);
            
            return MapToResponse(maintenanceRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing maintenance request {Id}", request.RequestId);
            throw;
        }
    }

    public async Task<bool> CompensateCancelRequestAsync(Guid id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            var maintenanceRequest = await _dataRepository.FindByIDAsync<MaintenanceRequest>(id, connection);
            if (maintenanceRequest == null)
            {
                return false;
            }
            
            maintenanceRequest.Status = MaintenanceStatus.Cancelled;
            var result = await _dataRepository.UpdateAsync(maintenanceRequest, maintenanceRequest.Id, connection);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling maintenance request {Id}", id);
            return false;
        }
    }

    private static MaintenanceRequestResponse MapToResponse(MaintenanceRequest req) => new()
    {
        Id = req.Id,
        RoomId = req.RoomId,
        Description = req.Description,
        AssignedStaffId = req.AssignedStaffId,
        RequestedAt = req.RequestedAt,
        CompletedAt = req.CompletedAt,
        Status = req.Status.ToString(),
        Notes = req.Notes
    };
}
