using HotelManagement.Services.Availability.Data;
using HotelManagement.Services.Availability.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services.Availability.Configuration;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AvailabilityDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AvailabilityDbContext>>();

        try
        {
            await SeedSeasonalPeriodsAsync(dbContext);
            await SeedPricingRulesAsync(dbContext);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedSeasonalPeriodsAsync(AvailabilityDbContext dbContext)
    {
        if (await dbContext.SeasonalPeriods.AnyAsync())
        {
            return;
        }

        // Sample seasonal periods for demonstration
        var hotelId = Guid.NewGuid(); // This should be a real hotel ID from your system
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

        await dbContext.SeasonalPeriods.AddRangeAsync(seasons);
    }

    private static async Task SeedPricingRulesAsync(AvailabilityDbContext dbContext)
    {
        if (await dbContext.PricingRules.AnyAsync())
        {
            return;
        }

        // Sample pricing rules for demonstration
        var hotelId = Guid.NewGuid(); // This should be a real hotel ID from your system
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
                DaysOfWeek = "5,6", // Friday and Saturday
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
                DaysOfWeek = "7", // Sunday
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
                DaysOfWeek = "1,2,3,4,5,6,7", // All days
                AdjustmentPercentage = -15.0m,
                Priority = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await dbContext.PricingRules.AddRangeAsync(rules);
    }
}
