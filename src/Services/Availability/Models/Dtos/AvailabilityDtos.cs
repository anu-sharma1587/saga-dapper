using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.Availability.Models.Dtos;

public record AvailabilityRequest
{
    [Required]
    public Guid HotelId { get; init; }

    [Required]
    public DateTime CheckIn { get; init; }

    [Required]
    public DateTime CheckOut { get; init; }

    public List<Guid>? RoomTypeIds { get; init; }
}

public record RoomAvailabilityResponse
{
    public Guid RoomTypeId { get; init; }
    public DateTime Date { get; init; }
    public int AvailableRooms { get; init; }
    public decimal CurrentPrice { get; init; }
}

public record CreatePricingRuleRequest
{
    [Required]
    public Guid HotelId { get; init; }

    public Guid? RoomTypeId { get; init; }

    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Required]
    public DateTime StartDate { get; init; }

    [Required]
    public DateTime EndDate { get; init; }

    public string DaysOfWeek { get; init; } = "1,2,3,4,5,6,7";

    [Range(-100, 100)]
    public decimal AdjustmentPercentage { get; init; }

    [Range(1, 100)]
    public int Priority { get; init; }
}

public record CreateInventoryBlockRequest
{
    [Required]
    public Guid HotelId { get; init; }

    [Required]
    public Guid RoomTypeId { get; init; }

    [Required]
    public DateTime StartDate { get; init; }

    [Required]
    public DateTime EndDate { get; init; }

    [Range(1, int.MaxValue)]
    public int BlockedRooms { get; init; }

    [Required]
    public string Reason { get; init; } = string.Empty;

    public string Reference { get; init; } = string.Empty;
}


public record UpdatePricingRuleRequest : CreatePricingRuleRequest
{
    public Guid Id { get; init; }
}

public record UpdateInventoryBlockRequest : CreateInventoryBlockRequest
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
}

public record UpdateSeasonalPeriodRequest : CreateSeasonalPeriodRequest
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
}

public record UpdateSpecialEventRequest : CreateSpecialEventRequest
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
}

public record BatchUpdateAvailabilityRequest
{
    public List<UpdateAvailabilityRequest> Updates { get; init; } = new();
}

public record CreateSeasonalPeriodRequest
{
    [Required]
    public Guid HotelId { get; init; }

    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Required]
    public DateTime StartDate { get; init; }

    [Required]
    public DateTime EndDate { get; init; }

    [Range(-100, 100)]
    public decimal BaseAdjustmentPercentage { get; init; }
}

public record CreateSpecialEventRequest
{
    [Required]
    public Guid HotelId { get; init; }

    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Required]
    public DateTime StartDate { get; init; }

    [Required]
    public DateTime EndDate { get; init; }

    [Range(-100, 100)]
    public decimal ImpactPercentage { get; init; }

    [Range(0, 1000)]
    public int ExpectedDemandIncrease { get; init; }
}

public record UpdateAvailabilityRequest
{
    [Required]
    public Guid HotelId { get; init; }

    [Required]
    public Guid RoomTypeId { get; init; }

    [Required]
    public DateTime Date { get; init; }

    [Range(0, int.MaxValue)]
    public int AvailableRooms { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? BasePrice { get; init; }
}

public record PricingAnalysisResponse
{
    public Guid RoomTypeId { get; init; }
    public DateTime Date { get; init; }
    public decimal BasePrice { get; init; }
    public decimal CurrentPrice { get; init; }
    public List<PriceAdjustmentFactor> Factors { get; init; } = new();
}
