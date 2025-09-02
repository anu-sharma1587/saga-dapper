using HotelManagement.Services.Availability.Models.Dtos;

namespace HotelManagement.Services.Availability.Models;

public class PriceAnalysisRequest
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
}
