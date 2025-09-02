using HotelManagement.Services.Loyalty.Data;
using HotelManagement.Services.Loyalty.DTOs;
using HotelManagement.Services.Loyalty.Models;
using HotelManagement.Services.Loyalty.SpInput;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services.Loyalty.Services;

public class LoyaltyService : ILoyaltyService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<LoyaltyService> _logger;

    public LoyaltyService(IDataRepository dataRepository, ILogger<LoyaltyService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<LoyaltyAccountResponse> AddPointsAsync(AddPointsRequest request)
    {
        try
        {
            var parameters = new AddPointsParams
            {
                GuestId = request.GuestId,
                Points = request.Points,
                Reason = request.Reason,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<LoyaltyAccount, AddPointsParams>(parameters);
            result.Tier = CalculateTier(result.Points);
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding loyalty points");
            throw;
        }
    }

    public async Task<LoyaltyAccountResponse> RedeemPointsAsync(RedeemPointsRequest request)
    {
        try
        {
            var parameters = new RedeemPointsParams
            {
                GuestId = request.GuestId,
                Points = request.Points,
                Reason = request.Reason,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<LoyaltyAccount, RedeemPointsParams>(parameters);
            result.Tier = CalculateTier(result.Points);
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming loyalty points");
            throw;
        }
    }

    public async Task<LoyaltyAccountResponse?> GetAccountByGuestIdAsync(Guid guestId)
    {
        try
        {
            var parameters = new GetAccountByGuestIdParams { GuestId = guestId };
            var result = await _dataRepository.QueryFirstOrDefaultStoredProcedureAsync<LoyaltyAccount, GetAccountByGuestIdParams>(parameters);
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loyalty account by guest id {GuestId}", guestId);
            return null;
        }
    }

    public async Task<bool> CompensateRevertPointsAsync(Guid guestId, int points, string reason)
    {
        try
        {
            var parameters = new CompensateRevertPointsParams
            {
                GuestId = guestId,
                Points = points,
                Reason = reason,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<CompensateRevertPointsParams>(parameters);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting loyalty points for guest {GuestId}", guestId);
            return false;
        }
    }

    private static LoyaltyTier CalculateTier(int points) => points switch
    {
        >= 50000 => LoyaltyTier.Diamond,
        >= 25000 => LoyaltyTier.Platinum,
        >= 10000 => LoyaltyTier.Gold,
        >= 5000 => LoyaltyTier.Silver,
        _ => LoyaltyTier.Standard
    };

    private static LoyaltyAccountResponse MapToResponse(LoyaltyAccount a) => new()
    {
        Id = a.Id,
        GuestId = a.GuestId,
        Points = a.Points,
        Tier = a.Tier.ToString(),
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
