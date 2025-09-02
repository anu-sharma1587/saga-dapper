namespace HotelManagement.Services.Availability.Models.Dtos;

public class UpdateAvailabilityRequest
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal? BasePrice { get; set; }
}

public class BatchUpdateAvailabilityRequest
{
    public List<UpdateAvailabilityRequest> Updates { get; set; } = new();
}
