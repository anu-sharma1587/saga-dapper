namespace HotelManagement.Services.Availability.Models;

public class RoomAvailability
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalRooms { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class PricingRule
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid? RoomTypeId { get; set; } // Null means applies to all room types
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DaysOfWeek { get; set; } = "1,2,3,4,5,6,7"; // Comma-separated days (1=Monday)
    public decimal AdjustmentPercentage { get; set; } // Positive for increase, negative for discount
    public int Priority { get; set; } // Higher priority rules are applied first
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class InventoryBlock
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BlockedRooms { get; set; }
    public string Reason { get; set; } = string.Empty; // e.g., Maintenance, Group Booking, Channel Allocation
    public string Reference { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SeasonalPeriod
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., Summer Season, Winter Season
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal BaseAdjustmentPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DemandForecast
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int ExpectedDemand { get; set; }
    public decimal SuggestedPriceAdjustment { get; set; }
    public string Factors { get; set; } = string.Empty; // JSON string of contributing factors
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SpecialEvent
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal ImpactPercentage { get; set; }
    public int ExpectedDemandIncrease { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
