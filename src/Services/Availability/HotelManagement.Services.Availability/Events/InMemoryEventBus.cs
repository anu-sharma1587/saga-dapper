using System.Threading.Channels;
using HotelManagement.Services.Availability.Events.Integration;

namespace HotelManagement.Services.Availability.Events;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
}

public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly Channel<object> _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public InMemoryEventBus(
        ILogger<InMemoryEventBus> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _channel = Channel.CreateUnbounded<object>();
        _cancellationTokenSource = new CancellationTokenSource();

        StartEventProcessing();
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        try
        {
            await _channel.Writer.WriteAsync(@event);
            _logger.LogInformation("Event {@EventType} published successfully", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {@EventType}", typeof(T).Name);
            throw;
        }
    }

    private void StartEventProcessing()
    {
        Task.Run(async () =>
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cancellationTokenSource.Token))
                {
                    if (_channel.Reader.TryRead(out var @event))
                    {
                        try
                        {
                            await ProcessEventAsync(@event);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing event {@EventType}", @event.GetType().Name);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Event processing stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event processing loop");
            }
        }, _cancellationTokenSource.Token);
    }

    private async Task ProcessEventAsync(object @event)
    {
        // Here we would typically handle the event based on its type
        // For now, we'll just log it
        switch (@event)
        {
            case RoomAvailabilityChangedEvent availabilityChanged:
                _logger.LogInformation(
                    "Room availability changed for hotel {HotelId}, room type {RoomTypeId}: {AvailableRooms} rooms available at {Price:C}",
                    availabilityChanged.HotelId,
                    availabilityChanged.RoomTypeId,
                    availabilityChanged.AvailableRooms,
                    availabilityChanged.CurrentPrice);
                break;

            case PriceChangedEvent priceChanged:
                _logger.LogInformation(
                    "Price changed for hotel {HotelId}, room type {RoomTypeId}: {OldPrice:C} -> {NewPrice:C}, Reason: {Reason}",
                    priceChanged.HotelId,
                    priceChanged.RoomTypeId,
                    priceChanged.OldPrice,
                    priceChanged.NewPrice,
                    priceChanged.Reason);
                break;

            case InventoryBlockCreatedEvent blockCreated:
                _logger.LogInformation(
                    "Inventory block created for hotel {HotelId}, room type {RoomTypeId}: {BlockedRooms} rooms blocked from {StartDate:d} to {EndDate:d}",
                    blockCreated.HotelId,
                    blockCreated.RoomTypeId,
                    blockCreated.BlockedRooms,
                    blockCreated.StartDate,
                    blockCreated.EndDate);
                break;

            case SeasonalPeriodCreatedEvent seasonCreated:
                _logger.LogInformation(
                    "Seasonal period '{Name}' created for hotel {HotelId}: {Adjustment:P} adjustment from {StartDate:d} to {EndDate:d}",
                    seasonCreated.Name,
                    seasonCreated.HotelId,
                    seasonCreated.BaseAdjustmentPercentage / 100,
                    seasonCreated.StartDate,
                    seasonCreated.EndDate);
                break;

            case SpecialEventCreatedEvent eventCreated:
                _logger.LogInformation(
                    "Special event '{Name}' created for hotel {HotelId}: {Impact:P} price impact, expecting {DemandIncrease:P} demand increase",
                    eventCreated.Name,
                    eventCreated.HotelId,
                    eventCreated.ImpactPercentage / 100,
                    eventCreated.ExpectedDemandIncrease / 100.0);
                break;
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
