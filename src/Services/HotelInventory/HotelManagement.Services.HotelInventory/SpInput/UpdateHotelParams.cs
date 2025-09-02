using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class UpdateHotelParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_hotel";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
}
