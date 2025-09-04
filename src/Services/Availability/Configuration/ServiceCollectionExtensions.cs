using HotelManagement.Services.Availability.Services;
using HotelManagement.Services.Availability.Events;


namespace HotelManagement.Services.Availability.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAvailabilityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "availability-db",
                tags: new[] { "ready", "db", "postgres" });

        return services;
    }
}
