namespace HotelManagement.Services.Availability.Models.Dtos;

public class CreatePricingRuleRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid HotelId { get; set; }
    public Guid? RoomTypeId { get; set; }
    public string DaysOfWeek { get; set; } = "1,2,3,4,5,6,7"; // Default to all days
    public decimal AdjustmentPercentage { get; set; }
    public int Priority { get; set; }
}

public class UpdatePricingRuleRequest : CreatePricingRuleRequest
{
    public Guid Id { get; set; }
}
