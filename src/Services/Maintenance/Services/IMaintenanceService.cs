using HotelManagement.Services.Maintenance.DTOs;

namespace HotelManagement.Services.Maintenance.Services;

public interface IMaintenanceService
{
    Task<MaintenanceRequestResponse> CreateRequestAsync(CreateRequestDto request);
    Task<MaintenanceRequestResponse?> GetRequestByIdAsync(Guid id);
    Task<IEnumerable<MaintenanceRequestResponse>> GetRequestsByRoomIdAsync(Guid roomId);
    Task<MaintenanceRequestResponse> CompleteRequestAsync(CompleteRequestDto request);
    Task<bool> CompensateCancelRequestAsync(Guid id);
}
