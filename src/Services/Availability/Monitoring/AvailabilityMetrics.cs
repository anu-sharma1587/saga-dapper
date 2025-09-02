using System.Diagnostics.Metrics;

namespace HotelManagement.Services.Availability.Monitoring;

public class AvailabilityMetrics
{
    private readonly Counter<int> _availabilityQueriesTotal;
    private readonly Counter<int> _priceUpdatesTotal;
    private readonly Counter<int> _inventoryBlocksCreated;
    private readonly Counter<int> _inventoryBlocksRemoved;
    private readonly Histogram<double> _priceCalculationDuration;
    private readonly Histogram<double> _cacheHitRatio;
    private readonly Histogram<int> _availableRoomsCount;

    public AvailabilityMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("HotelManagement.Availability");

        _availabilityQueriesTotal = meter.CreateCounter<int>(
            "availability_queries_total",
            description: "Total number of availability queries");

        _priceUpdatesTotal = meter.CreateCounter<int>(
            "price_updates_total",
            description: "Total number of price updates");

        _inventoryBlocksCreated = meter.CreateCounter<int>(
            "inventory_blocks_created_total",
            description: "Total number of inventory blocks created");

        _inventoryBlocksRemoved = meter.CreateCounter<int>(
            "inventory_blocks_removed_total",
            description: "Total number of inventory blocks removed");

        _priceCalculationDuration = meter.CreateHistogram<double>(
            "price_calculation_duration_seconds",
            unit: "s",
            description: "Duration of price calculations");

        _cacheHitRatio = meter.CreateHistogram<double>(
            "cache_hit_ratio",
            description: "Cache hit ratio for availability queries");

        _availableRoomsCount = meter.CreateHistogram<int>(
            "available_rooms",
            description: "Distribution of available rooms count");
    }

    public void RecordAvailabilityQuery(Guid hotelId)
    {
        _availabilityQueriesTotal.Add(1, new KeyValuePair<string, object?>("hotel_id", hotelId));
    }

    public void RecordPriceUpdate(Guid hotelId, Guid roomTypeId, decimal oldPrice, decimal newPrice)
    {
        _priceUpdatesTotal.Add(1, new KeyValuePair<string, object?>[]
        {
            new("hotel_id", hotelId),
            new("room_type_id", roomTypeId),
            new("price_change", newPrice - oldPrice)
        });
    }

    public void RecordInventoryBlockCreated(Guid hotelId, int blockedRooms)
    {
        _inventoryBlocksCreated.Add(1, new KeyValuePair<string, object?>[]
        {
            new("hotel_id", hotelId),
            new("blocked_rooms", blockedRooms)
        });
    }

    public void RecordInventoryBlockRemoved(Guid hotelId, int unblockedRooms)
    {
        _inventoryBlocksRemoved.Add(1, new KeyValuePair<string, object?>[]
        {
            new("hotel_id", hotelId),
            new("unblocked_rooms", unblockedRooms)
        });
    }

    public void RecordPriceCalculationDuration(TimeSpan duration, Guid hotelId)
    {
        _priceCalculationDuration.Record(duration.TotalSeconds, new KeyValuePair<string, object?>("hotel_id", hotelId));
    }

    public void RecordCacheHitRatio(double hitRatio, string cacheType)
    {
        _cacheHitRatio.Record(hitRatio, new KeyValuePair<string, object?>("cache_type", cacheType));
    }

    public void RecordAvailableRooms(int count, Guid hotelId, Guid roomTypeId)
    {
        _availableRoomsCount.Record(count, new KeyValuePair<string, object?>[]
        {
            new("hotel_id", hotelId),
            new("room_type_id", roomTypeId)
        });
    }
}
