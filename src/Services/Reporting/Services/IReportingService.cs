using HotelManagement.Services.Reporting.DTOs;

namespace HotelManagement.Services.Reporting.Services;

public interface IReportingService
{
    Task<ReportJobResponse> CreateReportAsync(CreateReportRequest request);
    Task<ReportJobResponse?> GetReportByIdAsync(Guid id);
    Task<IEnumerable<ReportJobResponse>> GetReportsByTypeAsync(string type);
    Task<bool> CompensateCancelReportAsync(Guid id);
}
