namespace HotelManagement.Services.Availability.Models.Dtos;

public class CreateSeasonalPeriodRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid HotelId { get; set; }
    public decimal BaseAdjustmentPercentage { get; set; }
}

public class UpdateSeasonalPeriodRequest : CreateSeasonalPeriodRequest
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
