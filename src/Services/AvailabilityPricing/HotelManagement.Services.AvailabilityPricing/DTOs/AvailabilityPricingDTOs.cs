namespace HotelManagement.Services.AvailabilityPricing.DTOs;

public class CreateAvailabilityPricingRequest
{
    public Guid HotelId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal PricePerNight { get; set; }
    public string? RoomType { get; set; }
}

public class AvailabilityPricingResponse
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal PricePerNight { get; set; }
    public string? RoomType { get; set; }
    public string? Status { get; set; }
    public string? Error { get; set; }
}
