using HotelManagement.Services.Availability.Models;
using DataAccess.Dapper;
using Dapper;
using Npgsql;

namespace HotelManagement.Services.Availability.Configuration;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

        try
        {
            using var connection = connectionFactory.CreateConnection();
            await SeedSeasonalPeriodsAsync(connection);
            await SeedPricingRulesAsync(connection);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedSeasonalPeriodsAsync(NpgsqlConnection connection)
    {
        var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"SeasonalPeriods\"");
        if (count > 0)
        {
            return;
        }

        var hotelId = Guid.NewGuid();
        var seasons = new[]
        {
            new SeasonalPeriod
            {
                Id = Guid.NewGuid(),
                Name = "Summer Peak Season",
                Description = "Peak summer travel season",
                StartDate = new DateTime(DateTime.Now.Year, 6, 1),
                EndDate = new DateTime(DateTime.Now.Year, 8, 31),
                HotelId = hotelId,
                BaseAdjustmentPercentage = 25.0m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new SeasonalPeriod
            {
                Id = Guid.NewGuid(),
                Name = "Winter Holiday Season",
                Description = "Holiday season with increased rates",
                StartDate = new DateTime(DateTime.Now.Year, 12, 15),
                EndDate = new DateTime(DateTime.Now.Year, 1, 5),
                HotelId = hotelId,
                BaseAdjustmentPercentage = 30.0m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new SeasonalPeriod
            {
                Id = Guid.NewGuid(),
                Name = "Off Peak Season",
                Description = "Low season with reduced rates",
                StartDate = new DateTime(DateTime.Now.Year, 1, 6),
                EndDate = new DateTime(DateTime.Now.Year, 3, 31),
                HotelId = hotelId,
                BaseAdjustmentPercentage = -15.0m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var season in seasons)
        {
            await connection.ExecuteAsync(
                "INSERT INTO \"SeasonalPeriods\" (\"Id\", \"Name\", \"Description\", \"StartDate\", \"EndDate\", \"HotelId\", \"BaseAdjustmentPercentage\", \"IsActive\", \"CreatedAt\") " +
                "VALUES (@Id, @Name, @Description, @StartDate, @EndDate, @HotelId, @BaseAdjustmentPercentage, @IsActive, @CreatedAt)",
                season);
        }
    }

    private static async Task SeedPricingRulesAsync(NpgsqlConnection connection)
    {
        var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"PricingRules\"");
        if (count > 0)
        {
            return;
        }

        var hotelId = Guid.NewGuid();
        var rules = new[]
        {
            new PricingRule
            {
                Id = Guid.NewGuid(),
                Name = "Weekend Premium",
                Description = "Higher rates for Friday and Saturday nights",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.AddYears(1).Date,
                HotelId = hotelId,
                DaysOfWeek = "5,6",
                AdjustmentPercentage = 20.0m,
                Priority = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new PricingRule
            {
                Id = Guid.NewGuid(),
                Name = "Sunday Special",
                Description = "Reduced rates for Sunday nights",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.AddYears(1).Date,
                HotelId = hotelId,
                DaysOfWeek = "7",
                AdjustmentPercentage = -10.0m,
                Priority = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new PricingRule
            {
                Id = Guid.NewGuid(),
                Name = "Extended Stay Discount",
                Description = "Discount for stays of 7 nights or more",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.AddYears(1).Date,
                HotelId = hotelId,
                DaysOfWeek = "1,2,3,4,5,6,7",
                AdjustmentPercentage = -15.0m,
                Priority = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var rule in rules)
        {
            await connection.ExecuteAsync(
                "INSERT INTO \"PricingRules\" (\"Id\", \"Name\", \"Description\", \"StartDate\", \"EndDate\", \"HotelId\", \"DaysOfWeek\", \"AdjustmentPercentage\", \"Priority\", \"IsActive\", \"CreatedAt\") " +
                "VALUES (@Id, @Name, @Description, @StartDate, @EndDate, @HotelId, @DaysOfWeek, @AdjustmentPercentage, @Priority, @IsActive, @CreatedAt)",
                rule);
        }
    }
}
