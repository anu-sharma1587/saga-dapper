using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class DeleteHotelParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_delete_hotel";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
