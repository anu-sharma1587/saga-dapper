using DataAccess;
using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using HotelManagement.Services.Availability.Events;
using HotelManagement.Services.Availability.Events.Integration;
using Dapper;
using System.Data;
// Removed EF Core

namespace HotelManagement.Services.Availability.Services;

public class AvailabilityService : IAvailabilityService
{
    // Removed EF Core DbContext
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IDapperDataRepository _dapperRepo;
    private readonly ILogger<AvailabilityService> _logger;
    private readonly IEventBus _eventBus;

    public AvailabilityService(
        IDbConnectionFactory dbConnectionFactory,
        IDapperDataRepository dapperRepo,
        ILogger<AvailabilityService> logger,
        IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dapperRepo = dapperRepo;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<List<RoomAvailabilityResponse>> GetAvailabilityAsync(
        Guid hotelId,
        DateTime checkIn,
        DateTime checkOut,
        List<Guid>? roomTypeIds = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetRoomAvailabilityParams
            {
                HotelId = hotelId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                RoomTypeId = roomTypeIds != null && roomTypeIds.Count == 1 ? roomTypeIds[0] : null,
                p_refcur_1 = null
            };
            var availabilities = (await _dapperRepo.ExecuteSpQueryAsync<RoomAvailability, SpInput.GetRoomAvailabilityParams>(param, db)).ToList();

            // If you need to filter by multiple roomTypeIds, filter in-memory
            if (roomTypeIds != null && roomTypeIds.Any())
                availabilities = availabilities.Where(a => roomTypeIds.Contains(a.RoomTypeId)).ToList();

            return availabilities.Select(a => new RoomAvailabilityResponse
            {
                RoomTypeId = a.RoomTypeId,
                Date = a.Date,
                AvailableRooms = a.AvailableRooms,
                CurrentPrice = a.CurrentPrice
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting availability for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<bool> UpdateAvailabilityAsync(
        Guid hotelId,
        Guid roomTypeId,
        DateTime date,
        int availableRooms,
        decimal? basePrice = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();

            if (basePrice == null)
            {
                // Get current price if basePrice is not provided
                var existingAvailability = await db.QueryFirstOrDefaultAsync<RoomAvailability>(
                    "SELECT * FROM RoomAvailabilities WHERE HotelId = @HotelId AND RoomTypeId = @RoomTypeId AND Date = @Date",
                    new { HotelId = hotelId, RoomTypeId = roomTypeId, Date = date.Date });

                if (existingAvailability == null)
                {
                    throw new ArgumentException("Base price is required for new availability records");
                }

                // Calculate the current price based on the base price and pricing rules
                decimal currentPrice = await CalculateCurrentPriceAsync(new RoomAvailability
                {
                    HotelId = hotelId,
                    RoomTypeId = roomTypeId,
                    Date = date.Date,
                    BasePrice = existingAvailability.BasePrice
                });

                var param = new SpInput.UpdateAvailabilityParams
                {
                    HotelId = hotelId,
                    RoomTypeId = roomTypeId,
                    Date = date.Date,
                    AvailableRooms = availableRooms,
                    BasePrice = null,
                    CurrentPrice = currentPrice,
                    LastUpdated = DateTime.UtcNow,
                    p_refcur_1 = null
                };

                var result = (await _dapperRepo.ExecuteSpQueryAsync<RoomAvailability, SpInput.UpdateAvailabilityParams>(param, db)).FirstOrDefault();

                await _eventBus.PublishAsync(new RoomAvailabilityChangedEvent
                {
                    HotelId = hotelId,
                    RoomTypeId = roomTypeId,
                    Date = date,
                    AvailableRooms = availableRooms,
                    CurrentPrice = result.CurrentPrice,
                    ChangeTrigger = EventTypes.ChangeTrigger.InventoryUpdate
                });
            }
            else
            {
                // Calculate the current price based on the base price and pricing rules
                decimal currentPrice = await CalculateCurrentPriceAsync(new RoomAvailability
                {
                    HotelId = hotelId,
                    RoomTypeId = roomTypeId,
                    Date = date.Date,
                    BasePrice = basePrice.Value
                });

                var param = new SpInput.UpdateAvailabilityParams
                {
                    HotelId = hotelId,
                    RoomTypeId = roomTypeId,
                    Date = date.Date,
                    AvailableRooms = availableRooms,
                    BasePrice = basePrice,
                    CurrentPrice = currentPrice,
                    LastUpdated = DateTime.UtcNow,
                    p_refcur_1 = null
                };

                var result = (await _dapperRepo.ExecuteSpQueryAsync<RoomAvailability, SpInput.UpdateAvailabilityParams>(param, db)).FirstOrDefault();

                await _eventBus.PublishAsync(new RoomAvailabilityChangedEvent
                {
                    HotelId = hotelId,
                    RoomTypeId = roomTypeId,
                    Date = date,
                    AvailableRooms = availableRooms,
                    CurrentPrice = result.CurrentPrice,
                    ChangeTrigger = EventTypes.ChangeTrigger.PriceUpdate
                });
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability for hotel {HotelId}, room type {RoomTypeId}", hotelId, roomTypeId);
            throw;
        }
    }

    public async Task<PricingRule> CreatePricingRuleAsync(PricingRule rule)
    {
        try
        {
            rule.CreatedAt = DateTime.UtcNow;
            rule.IsActive = true;
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.CreatePricingRuleParams
            {
                HotelId = rule.HotelId,
                Name = rule.Name,
                Description = rule.Description,
                StartDate = rule.StartDate,
                EndDate = rule.EndDate,
                DaysOfWeek = rule.DaysOfWeek,
                AdjustmentPercentage = rule.AdjustmentPercentage,
                Priority = rule.Priority,
                IsActive = rule.IsActive,
                CreatedAt = rule.CreatedAt,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<PricingRule, SpInput.CreatePricingRuleParams>(param, db)).FirstOrDefault();
            // Recalculate prices for affected dates
            await RecalculatePricesForPeriodAsync(rule.HotelId, rule.StartDate, rule.EndDate, rule.RoomTypeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pricing rule for hotel {HotelId}", rule.HotelId);
            throw;
        }
    }

    public async Task<PricingRule> UpdatePricingRuleAsync(PricingRule rule)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.UpdatePricingRuleParams
            {
                Id = rule.Id,
                HotelId = rule.HotelId,
                Name = rule.Name,
                Description = rule.Description,
                StartDate = rule.StartDate,
                EndDate = rule.EndDate,
                DaysOfWeek = rule.DaysOfWeek,
                AdjustmentPercentage = rule.AdjustmentPercentage,
                Priority = rule.Priority,
                IsActive = rule.IsActive,
                UpdatedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<PricingRule, SpInput.UpdatePricingRuleParams>(param, db)).FirstOrDefault();
            // Recalculate prices for affected dates
            await RecalculatePricesForPeriodAsync(rule.HotelId, rule.StartDate, rule.EndDate, rule.RoomTypeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pricing rule {RuleId}", rule.Id);
            throw;
        }
    }

    public async Task<bool> DeletePricingRuleAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.DeletePricingRuleParams
            {
                Id = id,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<PricingRule, SpInput.DeletePricingRuleParams>(param, db)).FirstOrDefault();
            if (result == null)
                return false;
            // Recalculate prices for affected dates
            await RecalculatePricesForPeriodAsync(result.HotelId, result.StartDate, result.EndDate, result.RoomTypeId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pricing rule {RuleId}", id);
            throw;
        }
    }

    public async Task<List<PricingRule>> GetActivePricingRulesAsync(Guid hotelId, Guid? roomTypeId = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetActivePricingRulesParams
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                p_refcur_1 = null
            };
            var rules = await _dapperRepo.ExecuteSpQueryAsync<PricingRule, SpInput.GetActivePricingRulesParams>(param, db);
            return rules.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pricing rules for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<InventoryBlock> CreateInventoryBlockAsync(InventoryBlock block)
    {
        try
        {
            block.CreatedAt = DateTime.UtcNow;
            block.IsActive = true;

            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.CreateInventoryBlockParams
            {
                HotelId = block.HotelId,
                RoomTypeId = block.RoomTypeId,
                StartDate = block.StartDate,
                EndDate = block.EndDate,
                BlockedRooms = block.BlockedRooms,
                Reason = block.Reason,
                Reference = block.Reference,
                IsActive = block.IsActive,
                CreatedAt = block.CreatedAt,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<InventoryBlock, SpInput.CreateInventoryBlockParams>(param, db)).FirstOrDefault();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory block for hotel {HotelId}", block.HotelId);
            throw;
        }
    }

    public async Task<bool> UpdateInventoryBlockAsync(InventoryBlock block)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.UpdateInventoryBlockParams
            {
                Id = block.Id,
                HotelId = block.HotelId,
                RoomTypeId = block.RoomTypeId,
                StartDate = block.StartDate,
                EndDate = block.EndDate,
                BlockedRooms = block.BlockedRooms,
                Reason = block.Reason,
                Reference = block.Reference,
                IsActive = block.IsActive,
                UpdatedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<InventoryBlock, SpInput.UpdateInventoryBlockParams>(param, db)).FirstOrDefault();
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory block {BlockId}", block.Id);
            throw;
        }
    }

    public async Task<bool> DeleteInventoryBlockAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.DeleteInventoryBlockParams
            {
                Id = id,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<InventoryBlock, SpInput.DeleteInventoryBlockParams>(param, db)).FirstOrDefault();
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inventory block {BlockId}", id);
            throw;
        }
    }

    public async Task<List<InventoryBlock>> GetActiveInventoryBlocksAsync(Guid hotelId, Guid? roomTypeId = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetActiveInventoryBlocksParams
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                p_refcur_1 = null
            };
            var blocks = await _dapperRepo.ExecuteSpQueryAsync<InventoryBlock, SpInput.GetActiveInventoryBlocksParams>(param, db);
            return blocks.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory blocks for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<SeasonalPeriod> CreateSeasonalPeriodAsync(SeasonalPeriod period)
    {
        try
        {
            period.CreatedAt = DateTime.UtcNow;
            period.IsActive = true;

            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.CreateSeasonalPeriodParams
            {
                HotelId = period.HotelId,
                Name = period.Name,
                Description = period.Description,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                BaseAdjustmentPercentage = period.BaseAdjustmentPercentage,
                IsActive = period.IsActive,
                CreatedAt = period.CreatedAt,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<SeasonalPeriod, SpInput.CreateSeasonalPeriodParams>(param, db)).FirstOrDefault();

            await _eventBus.PublishAsync(new SeasonalPeriodCreatedEvent
            {
                HotelId = period.HotelId,
                Name = period.Name,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                BaseAdjustmentPercentage = period.BaseAdjustmentPercentage
            });

            // Recalculate prices for the seasonal period
            await RecalculatePricesForPeriodAsync(period.HotelId, period.StartDate, period.EndDate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seasonal period for hotel {HotelId}", period.HotelId);
            throw;
        }
    }

    public async Task<SeasonalPeriod> UpdateSeasonalPeriodAsync(SeasonalPeriod period)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            period.UpdatedAt = DateTime.UtcNow;
            var param = new SpInput.UpdateSeasonalPeriodParams
            {
                Id = period.Id,
                HotelId = period.HotelId,
                Name = period.Name,
                Description = period.Description,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                BaseAdjustmentPercentage = period.BaseAdjustmentPercentage,
                IsActive = period.IsActive,
                UpdatedAt = period.UpdatedAt.Value,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<SeasonalPeriod, SpInput.UpdateSeasonalPeriodParams>(param, db)).FirstOrDefault();
            if (result == null)
                throw new KeyNotFoundException($"Seasonal period with ID {period.Id} not found");

            // Recalculate prices for the seasonal period
            await RecalculatePricesForPeriodAsync(period.HotelId, period.StartDate, period.EndDate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating seasonal period {PeriodId}", period.Id);
            throw;
        }
    }

    public async Task<bool> DeleteSeasonalPeriodAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.DeleteSeasonalPeriodParams
            {
                Id = id,
                p_refcur_1 = null
            };
            var period = (await _dapperRepo.ExecuteSpQueryAsync<SeasonalPeriod, SpInput.DeleteSeasonalPeriodParams>(param, db)).FirstOrDefault();
            if (period == null)
                return false;

            // Recalculate prices for the seasonal period
            await RecalculatePricesForPeriodAsync(period.HotelId, period.StartDate, period.EndDate);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting seasonal period {PeriodId}", id);
            throw;
        }
    }

    public async Task<List<SeasonalPeriod>> GetActiveSeasonalPeriodsAsync(Guid hotelId)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetActiveSeasonalPeriodsParams
            {
                HotelId = hotelId,
                p_refcur_1 = null
            };
            var periods = await _dapperRepo.ExecuteSpQueryAsync<SeasonalPeriod, SpInput.GetActiveSeasonalPeriodsParams>(param, db);
            return periods.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seasonal periods for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<SpecialEvent> CreateSpecialEventAsync(SpecialEvent specialEvent)
    {
        try
        {
            specialEvent.CreatedAt = DateTime.UtcNow;
            specialEvent.IsActive = true;

            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.CreateSpecialEventParams
            {
                HotelId = specialEvent.HotelId,
                Name = specialEvent.Name,
                Description = specialEvent.Description,
                StartDate = specialEvent.StartDate,
                EndDate = specialEvent.EndDate,
                ImpactPercentage = specialEvent.ImpactPercentage,
                ExpectedDemandIncrease = specialEvent.ExpectedDemandIncrease,
                IsActive = specialEvent.IsActive,
                CreatedAt = specialEvent.CreatedAt,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<SpecialEvent, SpInput.CreateSpecialEventParams>(param, db)).FirstOrDefault();

            await _eventBus.PublishAsync(new SpecialEventCreatedEvent
            {
                HotelId = specialEvent.HotelId,
                Name = specialEvent.Name,
                StartDate = specialEvent.StartDate,
                EndDate = specialEvent.EndDate,
                ImpactPercentage = specialEvent.ImpactPercentage,
                ExpectedDemandIncrease = specialEvent.ExpectedDemandIncrease
            });

            // Recalculate prices for the event period
            await RecalculatePricesForPeriodAsync(specialEvent.HotelId, specialEvent.StartDate, specialEvent.EndDate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating special event for hotel {HotelId}", specialEvent.HotelId);
            throw;
        }
    }

    public async Task<SpecialEvent> UpdateSpecialEventAsync(SpecialEvent specialEvent)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            specialEvent.UpdatedAt = DateTime.UtcNow;
            var param = new SpInput.UpdateSpecialEventParams
            {
                Id = specialEvent.Id,
                HotelId = specialEvent.HotelId,
                Name = specialEvent.Name,
                Description = specialEvent.Description,
                StartDate = specialEvent.StartDate,
                EndDate = specialEvent.EndDate,
                ImpactPercentage = specialEvent.ImpactPercentage,
                ExpectedDemandIncrease = specialEvent.ExpectedDemandIncrease,
                IsActive = specialEvent.IsActive,
                UpdatedAt = specialEvent.UpdatedAt.Value,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<SpecialEvent, SpInput.UpdateSpecialEventParams>(param, db)).FirstOrDefault();
            if (result == null)
                throw new KeyNotFoundException($"Special event with ID {specialEvent.Id} not found");

            // Recalculate prices for the event period
            await RecalculatePricesForPeriodAsync(specialEvent.HotelId, specialEvent.StartDate, specialEvent.EndDate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating special event {EventId}", specialEvent.Id);
            throw;
        }
    }

    public async Task<bool> DeleteSpecialEventAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.DeleteSpecialEventParams
            {
                Id = id,
                p_refcur_1 = null
            };
            var specialEvent = (await _dapperRepo.ExecuteSpQueryAsync<SpecialEvent, SpInput.DeleteSpecialEventParams>(param, db)).FirstOrDefault();
            if (specialEvent == null)
                return false;

            // Recalculate prices for the event period
            await RecalculatePricesForPeriodAsync(specialEvent.HotelId, specialEvent.StartDate, specialEvent.EndDate);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting special event {EventId}", id);
            throw;
        }
    }

    public async Task<List<SpecialEvent>> GetActiveSpecialEventsAsync(Guid hotelId)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetActiveSpecialEventsParams
            {
                HotelId = hotelId,
                p_refcur_1 = null
            };
            var events = await _dapperRepo.ExecuteSpQueryAsync<SpecialEvent, SpInput.GetActiveSpecialEventsParams>(param, db);
            return events.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting special events for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<PricingAnalysisResponse> GetPricingAnalysisAsync(
        Guid hotelId,
        Guid roomTypeId,
        DateTime date)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetPricingAnalysisParams
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                Date = date.Date,
                p_refcur_1 = null
            };
            var availability = (await _dapperRepo.ExecuteSpQueryAsync<RoomAvailability, SpInput.GetPricingAnalysisParams>(param, db)).FirstOrDefault();

            if (availability == null)
            {
                throw new KeyNotFoundException("No availability record found for the specified date");
            }

            var factors = new List<PriceAdjustmentFactor>();

            // Add seasonal adjustments
            var seasonalPeriods = (await db.QueryAsync<SeasonalPeriod>(
                "SELECT * FROM SeasonalPeriods WHERE HotelId = @HotelId AND IsActive = TRUE AND StartDate <= @Date AND EndDate >= @Date",
                new { HotelId = hotelId, Date = date })).ToList();

            foreach (var season in seasonalPeriods)
            {
                factors.Add(new PriceAdjustmentFactor
                {
                    Type = "SeasonalPeriod",
                    Name = season.Name,
                    Description = season.Description,
                    AdjustmentPercentage = season.BaseAdjustmentPercentage,
                    Priority = 10
                });
            }

            // Add pricing rules
            var pricingRules = (await db.QueryAsync<PricingRule>(
                "SELECT * FROM PricingRules WHERE HotelId = @HotelId AND IsActive = TRUE AND StartDate <= @Date AND EndDate >= @Date AND (RoomTypeId IS NULL OR RoomTypeId = @RoomTypeId) AND DaysOfWeek LIKE @DayOfWeek ORDER BY Priority",
                new { HotelId = hotelId, RoomTypeId = roomTypeId, Date = date, DayOfWeek = "%" + (((int)date.DayOfWeek + 6) % 7 + 1).ToString() + "%" })).ToList();

            foreach (var rule in pricingRules)
            {
                factors.Add(new PriceAdjustmentFactor
                {
                    Type = "PricingRule",
                    Name = rule.Name,
                    Description = rule.Description,
                    AdjustmentPercentage = rule.AdjustmentPercentage,
                    Priority = rule.Priority
                });
            }

            // Add special events
            var specialEvents = (await db.QueryAsync<SpecialEvent>(
                "SELECT * FROM SpecialEvents WHERE HotelId = @HotelId AND IsActive = TRUE AND StartDate <= @Date AND EndDate >= @Date",
                new { HotelId = hotelId, Date = date })).ToList();

            foreach (var specialEvent in specialEvents)
            {
                factors.Add(new PriceAdjustmentFactor
                {
                    Type = "SpecialEvent",
                    Name = specialEvent.Name,
                    Description = specialEvent.Description,
                    AdjustmentPercentage = specialEvent.ImpactPercentage,
                    Priority = 20
                });
            }

            // Add demand-based adjustments
            var forecast = await db.QueryFirstOrDefaultAsync<HotelManagement.Services.Availability.Models.Dtos.DemandForecast>(
                "SELECT * FROM DemandForecasts WHERE HotelId = @HotelId AND RoomTypeId = @RoomTypeId AND Date = @Date",
                new { HotelId = hotelId, RoomTypeId = roomTypeId, Date = date });

            if (forecast != null)
            {
                factors.Add(new PriceAdjustmentFactor
                {
                    Type = "DemandForecast",
                    Name = "Demand-based Adjustment",
                    Description = $"Based on expected demand of {forecast.ExpectedDemand} rooms",
                    AdjustmentPercentage = (decimal)forecast.SuggestedPriceAdjustment,
                    Priority = 30
                });
            }

            return new PricingAnalysisResponse
            {
                RoomTypeId = roomTypeId,
                Date = date,
                BasePrice = availability.BasePrice,
                CurrentPrice = availability.CurrentPrice,
                Factors = factors.OrderBy(f => f.Priority).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pricing analysis for hotel {HotelId}, room type {RoomTypeId}", hotelId, roomTypeId);
            throw;
        }
    }

    public async Task UpdateDemandForecastAsync(HotelManagement.Services.Availability.Models.Dtos.DemandForecast forecast)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.UpdateDemandForecastParams
            {
                HotelId = forecast.HotelId,
                RoomTypeId = forecast.RoomTypeId,
                Date = forecast.Date,
                ExpectedDemand = forecast.ExpectedDemand,
                SuggestedPriceAdjustment = (decimal)forecast.SuggestedPriceAdjustment,
                Factors = forecast.Factors,
                IsActive = forecast.IsActive,
                p_refcur_1 = null
            };
            var result = (await _dapperRepo.ExecuteSpQueryAsync<HotelManagement.Services.Availability.Models.Dtos.DemandForecast, SpInput.UpdateDemandForecastParams>(param, db)).FirstOrDefault();

            // Recalculate prices for the forecasted date
            await RecalculatePricesForPeriodAsync(forecast.HotelId, forecast.Date, forecast.Date, forecast.RoomTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating demand forecast for hotel {HotelId}, room type {RoomTypeId}",
                forecast.HotelId, forecast.RoomTypeId);
            throw;
        }
    }

    public async Task<List<HotelManagement.Services.Availability.Models.Dtos.DemandForecast>> GetDemandForecastsAsync(
        Guid hotelId,
        Guid roomTypeId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SpInput.GetDemandForecastsParams
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                StartDate = startDate,
                EndDate = endDate,
                p_refcur_1 = null
            };
            var forecasts = await _dapperRepo.ExecuteSpQueryAsync<HotelManagement.Services.Availability.Models.Dtos.DemandForecast, SpInput.GetDemandForecastsParams>(param, db);
            return forecasts.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting demand forecasts for hotel {HotelId}, room type {RoomTypeId}",
                hotelId, roomTypeId);
            throw;
        }
    }

    private async Task RecalculatePricesForPeriodAsync(Guid hotelId, DateTime startDate, DateTime endDate, Guid? roomTypeId = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var sql = "SELECT * FROM RoomAvailabilities WHERE HotelId = @HotelId AND Date >= @StartDate AND Date <= @EndDate";
            if (roomTypeId.HasValue)
                sql += " AND RoomTypeId = @RoomTypeId";
            var availabilities = (await db.QueryAsync<RoomAvailability>(sql, new { HotelId = hotelId, StartDate = startDate, EndDate = endDate, RoomTypeId = roomTypeId })).ToList();

            foreach (var availability in availabilities)
            {
                var oldPrice = availability.CurrentPrice;
                availability.CurrentPrice = await CalculateCurrentPriceAsync(availability);
                availability.LastUpdated = DateTime.UtcNow;

                await db.ExecuteAsync(
                    "UPDATE RoomAvailabilities SET CurrentPrice = @CurrentPrice, LastUpdated = @LastUpdated WHERE Id = @Id",
                    new { CurrentPrice = availability.CurrentPrice, LastUpdated = availability.LastUpdated, Id = availability.Id });

                if (oldPrice != availability.CurrentPrice)
                {
                    await _eventBus.PublishAsync(new PriceChangedEvent
                    {
                        HotelId = availability.HotelId,
                        RoomTypeId = availability.RoomTypeId,
                        Date = availability.Date,
                        OldPrice = oldPrice,
                        NewPrice = availability.CurrentPrice,
                        Reason = EventTypes.ChangeTrigger.Recalculation
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating prices for hotel {HotelId}", hotelId);
            throw;
        }
    }

    private async Task<decimal> CalculateCurrentPriceAsync(RoomAvailability availability)
    {
        var basePrice = availability.BasePrice;
        var date = availability.Date;
        var adjustments = new List<decimal>();

        // Apply seasonal adjustments
        using var db = await _dbConnectionFactory.CreateAsync();
        var seasons = (await db.QueryAsync<SeasonalPeriod>("SELECT * FROM SeasonalPeriods WHERE HotelId = @HotelId AND IsActive = TRUE AND StartDate <= @Date AND EndDate >= @Date", new { HotelId = availability.HotelId, Date = date })).ToList();

        foreach (var season in seasons)
        {
            adjustments.Add(season.BaseAdjustmentPercentage);
        }

        // Apply pricing rules
        var rules = (await db.QueryAsync<PricingRule>("SELECT * FROM PricingRules WHERE HotelId = @HotelId AND IsActive = TRUE AND StartDate <= @Date AND EndDate >= @Date AND (RoomTypeId IS NULL OR RoomTypeId = @RoomTypeId) AND DaysOfWeek LIKE @DayOfWeek ORDER BY Priority", new { HotelId = availability.HotelId, RoomTypeId = availability.RoomTypeId, Date = date, DayOfWeek = "%" + (((int)date.DayOfWeek + 6) % 7 + 1).ToString() + "%" })).ToList();

        foreach (var rule in rules)
        {
            adjustments.Add(rule.AdjustmentPercentage);
        }

        // Apply special event adjustments
        var events = (await db.QueryAsync<SpecialEvent>("SELECT * FROM SpecialEvents WHERE HotelId = @HotelId AND IsActive = TRUE AND StartDate <= @Date AND EndDate >= @Date", new { HotelId = availability.HotelId, Date = date })).ToList();

        foreach (var specialEvent in events)
        {
            adjustments.Add(specialEvent.ImpactPercentage);
        }

        // Apply demand-based adjustments
        var forecast = await db.QueryFirstOrDefaultAsync<HotelManagement.Services.Availability.Models.Dtos.DemandForecast>("SELECT * FROM DemandForecasts WHERE HotelId = @HotelId AND RoomTypeId = @RoomTypeId AND Date = @Date", new { HotelId = availability.HotelId, RoomTypeId = availability.RoomTypeId, Date = date });

        if (forecast != null)
        {
            adjustments.Add((decimal)forecast.SuggestedPriceAdjustment);
        }

        // Calculate final price with all adjustments
        var finalPrice = basePrice;
        foreach (var adjustment in adjustments)
        {
            finalPrice += finalPrice * (adjustment / 100);
        }

        // Round to 2 decimal places
        return Math.Round(finalPrice, 2);
    }
}
