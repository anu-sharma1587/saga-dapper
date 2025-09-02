using HotelManagement.Services.AvailabilityPricing.DTOs;
using HotelManagement.Services.AvailabilityPricing.Models;
using HotelManagement.Services.AvailabilityPricing.SpInput;
using DataAccess;

namespace HotelManagement.Services.AvailabilityPricing.Services;

public class AvailabilityPricingService : IAvailabilityPricingService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<AvailabilityPricingService> _logger;

    public AvailabilityPricingService(IDataRepository dataRepository, ILogger<AvailabilityPricingService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<AvailabilityPricingResponse> CreateAvailabilityPricingAsync(CreateAvailabilityPricingRequest request)
    {
        try
        {
            var parameters = new CreateAvailabilityPricingParams
            {
                Id = Guid.NewGuid(),
                HotelId = request.HotelId,
                Date = request.Date,
                AvailableRooms = request.AvailableRooms,
                PricePerNight = request.PricePerNight,
                RoomType = request.RoomType,
                Status = "Active"
            };

            var result = await _dataRepository.ExecuteStoredProcedureAsync<AvailabilityPricing, CreateAvailabilityPricingParams>(parameters);
            
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating availability pricing");
            throw;
        }
    }

    public async Task<AvailabilityPricingResponse?> GetAvailabilityPricingByIdAsync(Guid id)
    {
        try
        {
            var parameters = new GetAvailabilityPricingByIdParams { Id = id };
            var result = await _dataRepository.QueryFirstOrDefaultStoredProcedureAsync<AvailabilityPricing, GetAvailabilityPricingByIdParams>(parameters);
            
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting availability pricing by id {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<AvailabilityPricingResponse>> GetAvailabilityPricingsByHotelAsync(Guid hotelId)
    {
        try
        {
            var parameters = new GetAvailabilityPricingsByHotelParams { HotelId = hotelId };
            var results = await _dataRepository.QueryStoredProcedureAsync<AvailabilityPricing, GetAvailabilityPricingsByHotelParams>(parameters);
            
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting availability pricings by hotel {HotelId}", hotelId);
            return Enumerable.Empty<AvailabilityPricingResponse>();
        }
    }

    public async Task<bool> CompensateCancelAvailabilityPricingAsync(Guid id)
    {
        try
        {
            var parameters = new CompensateCancelAvailabilityPricingParams { Id = id };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<CompensateCancelAvailabilityPricingParams>(parameters);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error compensating cancel availability pricing {Id}", id);
            return false;
        }
    }

    private static AvailabilityPricingResponse MapToResponse(AvailabilityPricing ap) => new()
    {
        Id = ap.Id,
        HotelId = ap.HotelId,
        Date = ap.Date,
        AvailableRooms = ap.AvailableRooms,
        PricePerNight = ap.PricePerNight,
        RoomType = ap.RoomType,
        Status = ap.Status,
        Error = ap.Error
    };
}
