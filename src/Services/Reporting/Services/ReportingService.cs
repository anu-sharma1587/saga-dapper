using HotelManagement.Services.Reporting.DTOs;
using HotelManagement.Services.Reporting.Models;
using HotelManagement.Services.Reporting.SpInput;
using DataAccess;

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
            var param = new CreateReportParams
            {
                Id = Guid.NewGuid(),
                Type = request.Type,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<ReportJob, CreateReportParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new Exception("Failed to create report job");
            }

            return MapToResponse(result);
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
            var param = new GetReportByIdParams
            {
                Id = id,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<ReportJob, GetReportByIdParams>(param, db)).FirstOrDefault();
            
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
            var param = new GetReportsByTypeParams
            {
                Type = type,
                p_refcur_1 = null
            };

            var results = await _dapperRepo.ExecuteSpQueryAsync<ReportJob, GetReportsByTypeParams>(param, db);
            
            return results.Select(MapToResponse);
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
            var param = new CancelReportParams
            {
                Id = id,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<ReportJob, CancelReportParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling report job with ID {Id}", id);
            throw;
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
