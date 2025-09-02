using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class GetAllHotelsParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_all_hotels";
    public object? p_refcur_1 { get; set; }
    
    public bool IncludeInactive { get; set; }
}
