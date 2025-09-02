namespace HotelManagement.Services.Availability.Models.Dtos;

public class CreateSpecialEventRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid HotelId { get; set; }
    public decimal ImpactPercentage { get; set; }
    public int ExpectedDemandIncrease { get; set; }
}

public class UpdateSpecialEventRequest : CreateSpecialEventRequest
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
