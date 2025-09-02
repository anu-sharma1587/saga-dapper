namespace HotelManagement.Services.Availability.Models.Dtos;

public class UpdateDemandForecastRequest
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int ExpectedDemand { get; set; }
    public decimal SuggestedPriceAdjustment { get; set; }
    public Dictionary<string, decimal> Factors { get; set; } = new();
}
