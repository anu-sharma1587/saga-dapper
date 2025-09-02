using System.Diagnostics;

namespace HotelManagement.Services.Availability.Monitoring;

public static class ActivitySourceProvider
{
    public static readonly ActivitySource Source = new("HotelManagement.Availability");
}
