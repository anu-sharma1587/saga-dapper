namespace HotelManagement.Services.Availability.Models.Dtos;

public record PriceAdjustmentFactor
{
    public string Type { get; init; } = string.Empty; // e.g., "SeasonalPeriod", "PricingRule", "DemandForecast"
    public string Name { get; init; } = string.Empty;
    public decimal AdjustmentPercentage { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Priority { get; init; }
}
