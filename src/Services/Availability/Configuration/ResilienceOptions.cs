using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.Availability.Configuration;

public class ResilienceOptions
{
    [Range(1, 5)]
    public int MaxRetries { get; set; } = 3;

    [Range(1, 100)]
    public int HandledEventsAllowedBeforeBreaking { get; set; } = 5;

    [Range(1, 300)]
    public int DurationOfBreakInSeconds { get; set; } = 30;

    [Range(1, 30)]
    public int TimeoutInSeconds { get; set; } = 10;
}
