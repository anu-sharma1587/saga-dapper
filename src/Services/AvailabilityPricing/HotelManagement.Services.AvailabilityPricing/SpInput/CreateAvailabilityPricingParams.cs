using DataAccess;

namespace HotelManagement.Services.AvailabilityPricing.SpInput;

public class CreateAvailabilityPricingParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal PricePerNight { get; set; }
    public string? RoomType { get; set; }
    public string Status { get; set; } = "Active";

    public string StoredProcedureName => "sp_CreateAvailabilityPricing";
}
