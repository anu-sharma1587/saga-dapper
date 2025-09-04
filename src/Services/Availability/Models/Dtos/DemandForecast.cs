using System;

namespace HotelManagement.Services.Availability.Models.Dtos
{
    public class DemandForecast
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public double ForecastValue { get; set; }
        public int ExpectedDemand { get; set; }
        public double SuggestedPriceAdjustment { get; set; }
        public string Factors { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Type { get; set; } = "DemandForecast";
    }
}
