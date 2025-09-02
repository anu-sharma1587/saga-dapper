using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class SearchHotelsParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_search_hotels";
    public object? p_refcur_1 { get; set; }
    
    public string SearchTerm { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? StarRating { get; set; }
    public bool IncludeInactive { get; set; } = false;
}
