using HealthChecks.NpgSql;
using HotelManagement.Services.Availability.Data;
using HotelManagement.Services.Availability.Services;
using HotelManagement.Services.Availability.Events;
// Removed EF Core dependency

namespace HotelManagement.Services.Availability.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAvailabilityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AvailabilityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "availability-db",
                tags: new[] { "ready", "db", "postgres" });

        return services;
    }

    public static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AvailabilityDbContext>();
        dbContext.Database.Migrate();
    }
}
