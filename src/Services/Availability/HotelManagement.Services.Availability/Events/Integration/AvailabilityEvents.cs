namespace HotelManagement.Services.Availability.Events.Integration;

public record RoomAvailabilityChangedEvent
{
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime Date { get; init; }
    public int AvailableRooms { get; init; }
    public decimal CurrentPrice { get; init; }
    public string ChangeTrigger { get; init; } = string.Empty; // InventoryBlock, PricingRule, SeasonalPeriod, SpecialEvent, Manual
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record PriceChangedEvent
{
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime Date { get; init; }
    public decimal OldPrice { get; init; }
    public decimal NewPrice { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record InventoryBlockCreatedEvent
{
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int BlockedRooms { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record SeasonalPeriodCreatedEvent
{
    public Guid HotelId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal BaseAdjustmentPercentage { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record SpecialEventCreatedEvent
{
    public Guid HotelId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal ImpactPercentage { get; init; }
    public int ExpectedDemandIncrease { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
