using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class SearchGuestsParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_search_guests";
    public object? p_refcur_1 { get; set; }
    
    public string SearchTerm { get; set; } = null!;
}
