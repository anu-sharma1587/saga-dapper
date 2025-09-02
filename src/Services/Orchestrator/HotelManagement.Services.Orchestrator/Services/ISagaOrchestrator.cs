using HotelManagement.Services.Orchestrator.DTOs;

namespace HotelManagement.Services.Orchestrator.Services;

public interface ISagaOrchestrator
{
    Task<SagaResult> StartReservationSagaAsync(StartReservationSagaRequest request);
}
