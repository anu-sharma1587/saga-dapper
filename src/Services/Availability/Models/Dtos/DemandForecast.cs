using System;

namespace HotelManagement.Services.Availability.Models.Dtos
{
    public class DemandForecast
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public double ForecastValue { get; set; }
        public string Type { get; set; } = "DemandForecast";
    }
}
