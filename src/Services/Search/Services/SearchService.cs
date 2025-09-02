using HotelManagement.Services.Search.DTOs;
using HotelManagement.Services.Search.Models;
using HotelManagement.Services.Search.SpInput;
using DataAccess;

namespace HotelManagement.Services.Search.Services;

public class SearchService : ISearchService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IDapperDataRepository _dapperRepo;
    private readonly ILogger<SearchService> _logger;

    public SearchService(
        IDbConnectionFactory dbConnectionFactory,
        IDapperDataRepository dapperRepo,
        ILogger<SearchService> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dapperRepo = dapperRepo;
        _logger = logger;
    }

    public async Task<SearchQueryResponse> CreateSearchAsync(CreateSearchRequest request)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CreateSearchQueryParams
            {
                QueryText = request.QueryText,
                Type = request.Type ?? string.Empty,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending",
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<SearchQuery, CreateSearchQueryParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new Exception("Failed to create search query");
            }

            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating search query");
            throw;
        }
    }

    public async Task<SearchQueryResponse?> GetSearchByIdAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new GetSearchByIdParams
            {
                Id = id,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<SearchQuery, GetSearchByIdParams>(param, db)).FirstOrDefault();
            
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving search query with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<SearchQueryResponse>> GetSearchesByTypeAsync(string type)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new GetSearchesByTypeParams
            {
                Type = type,
                p_refcur_1 = null
            };

            var results = await _dapperRepo.ExecuteSpQueryAsync<SearchQuery, GetSearchesByTypeParams>(param, db);
            
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving search queries of type {Type}", type);
            throw;
        }
    }

    public async Task<bool> CompensateCancelSearchAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CancelSearchParams
            {
                Id = id,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<SearchQuery, CancelSearchParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling search query with ID {Id}", id);
            throw;
        }
    }

    private static SearchQueryResponse MapToResponse(SearchQuery query) => new()
    {
        Id = query.Id,
        QueryText = query.QueryText,
        Type = query.Type,
        RequestedAt = query.RequestedAt,
        Status = query.Status,
        ResultJson = query.ResultJson,
        Error = query.Error
    };
}
