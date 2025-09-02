using HotelManagement.Services.Availability.Models.Dtos;

namespace HotelManagement.Services.Availability.Models;

public class GetAvailabilityRequest
{
    public Guid HotelId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public List<Guid>? RoomTypeIds { get; set; }
}
