using HotelManagement.Services.Reporting.DTOs;
using HotelManagement.Services.Reporting.Models;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.Reporting.Services;

public class ReportingService : IReportingService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IDapperDataRepository _dapperRepo;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(
        IDbConnectionFactory dbConnectionFactory,
        IDapperDataRepository dapperRepo,
        ILogger<ReportingService> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dapperRepo = dapperRepo;
        _logger = logger;
    }

    public async Task<ReportJobResponse> CreateReportAsync(CreateReportRequest request)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reportJob = new ReportJob
            {
                Id = Guid.NewGuid(),
                Type = request.Type,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            await _dapperRepo.AddAsync(reportJob, db);
            
            return MapToResponse(reportJob);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report job");
            throw;
        }
    }

    public async Task<ReportJobResponse?> GetReportByIdAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var result = await _dapperRepo.FindByIDAsync<ReportJob>(id, db);
            
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report job with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ReportJobResponse>> GetReportsByTypeAsync(string type)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetReportsByTypeAsync not fully implemented with current interface");
            return Enumerable.Empty<ReportJobResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report jobs of type {Type}", type);
            throw;
        }
    }

    public async Task<bool> CompensateCancelReportAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reportJob = await _dapperRepo.FindByIDAsync<ReportJob>(id, db);
            if (reportJob == null)
            {
                return false;
            }

            reportJob.Status = "Cancelled";
            var result = await _dapperRepo.UpdateAsync(reportJob, reportJob.Id, db);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling report job with ID {Id}", id);
            return false;
        }
    }

    private static ReportJobResponse MapToResponse(ReportJob job) => new()
    {
        Id = job.Id,
        Type = job.Type,
        Status = job.Status,
        RequestedAt = job.RequestedAt,
        CompletedAt = job.CompletedAt,
        ResultUrl = job.ResultUrl,
        Error = job.Error
    };
}
