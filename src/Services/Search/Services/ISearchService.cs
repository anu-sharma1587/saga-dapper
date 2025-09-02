using HotelManagement.Services.Search.DTOs;

namespace HotelManagement.Services.Search.Services;

public interface ISearchService
{
    Task<SearchQueryResponse> CreateSearchAsync(CreateSearchRequest request);
    Task<SearchQueryResponse?> GetSearchByIdAsync(Guid id);
    Task<IEnumerable<SearchQueryResponse>> GetSearchesByTypeAsync(string type);
    Task<bool> CompensateCancelSearchAsync(Guid id);
}
