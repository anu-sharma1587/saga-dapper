using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class AddRoomTypeParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_add_room_type";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxOccupancy { get; set; }
    public int TotalRooms { get; set; }
    public decimal BasePrice { get; set; }
    public int SizeSqft { get; set; }
    public string BedConfiguration { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
