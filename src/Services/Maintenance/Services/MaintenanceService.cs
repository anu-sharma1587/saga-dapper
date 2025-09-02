using HotelManagement.Services.Maintenance.DTOs;
using HotelManagement.Services.Maintenance.Models;
using HotelManagement.Services.Maintenance.SpInput;
using DataAccess;

namespace HotelManagement.Services.Maintenance.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<MaintenanceService> _logger;

    public MaintenanceService(IDataRepository dataRepository, ILogger<MaintenanceService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<MaintenanceRequestResponse> CreateRequestAsync(CreateRequestDto request)
    {
        try
        {
            var parameters = new CreateRequestParams
            {
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                Description = request.Description,
                AssignedStaffId = request.AssignedStaffId,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending",
                Notes = request.Notes
            };

            var result = await _dataRepository.ExecuteStoredProcedureAsync<MaintenanceRequest, CreateRequestParams>(parameters);
            
            return MapToResponse(result);
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
            var parameters = new GetRequestByIdParams { Id = id };
            var result = await _dataRepository.QueryFirstOrDefaultStoredProcedureAsync<MaintenanceRequest, GetRequestByIdParams>(parameters);
            
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
            var parameters = new GetRequestsByRoomIdParams { RoomId = roomId };
            var results = await _dataRepository.QueryStoredProcedureAsync<MaintenanceRequest, GetRequestsByRoomIdParams>(parameters);
            
            return results.Select(MapToResponse);
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
            var parameters = new CompleteRequestParams 
            { 
                Id = request.RequestId,
                CompletedAt = DateTime.UtcNow,
                Status = "Completed",
                Notes = request.Notes
            };
            
            var result = await _dataRepository.ExecuteStoredProcedureAsync<MaintenanceRequest, CompleteRequestParams>(parameters);
            
            return MapToResponse(result);
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
            var parameters = new CompensateCancelRequestParams 
            { 
                Id = id,
                Status = "Cancelled"
            };
            
            var result = await _dataRepository.ExecuteStoredProcedureAsync<CompensateCancelRequestParams>(parameters);
            
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
