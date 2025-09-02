using HotelManagement.Services.Guest.DTOs;
using HotelManagement.Services.Guest.Models;
using HotelManagement.Services.Guest.SpInput;
using HotelManagement.BuildingBlocks.Common.Exceptions;
using DataAccess;

namespace HotelManagement.Services.Guest.Services;

public class GuestService : IGuestService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IDapperDataRepository _dapperRepo;
    private readonly ILogger<GuestService> _logger;

    public GuestService(IDbConnectionFactory dbConnectionFactory, IDapperDataRepository dapperRepo, ILogger<GuestService> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dapperRepo = dapperRepo;
        _logger = logger;
    }

    public async Task<GuestProfileResponse> CreateGuestProfileAsync(CreateGuestProfileRequest request)
    {
        try
        {
            // First check if a profile already exists for this user
            try
            {
                var existingProfile = await GetGuestProfileByUserIdAsync(request.UserId);
                if (existingProfile != null)
                {
                    throw new BusinessException("Guest profile already exists for this user.");
                }
            }
            catch (NotFoundException)
            {
                // This is expected - no profile exists yet
            }

            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CreateGuestProfileParams
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality,
                PassportNumber = request.PassportNumber,
                PassportExpiryDate = request.PassportExpiryDate,
                PreferredLanguage = request.PreferredLanguage,
                NewsletterSubscribed = request.NewsletterSubscribed,
                CreatedAt = DateTime.UtcNow,
                LoyaltyTier = LoyaltyTier.Standard.ToString(),
                LoyaltyPoints = 0,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, CreateGuestProfileParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new Exception("Failed to create guest profile");
            }

            // Handle preferences if they exist
            if (request.Preferences != null && request.Preferences.Any())
            {
                foreach (var preferenceRequest in request.Preferences)
                {
                    await AddPreferenceAsync(result.Id, preferenceRequest);
                }
                
                // Refresh the guest profile to include the new preferences
                result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, GetGuestProfileByIdParams>(
                    new GetGuestProfileByIdParams { Id = result.Id, p_refcur_1 = null }, db)).FirstOrDefault();
            }

            return MapToResponse(result);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating guest profile");
            throw;
        }
    }

    public async Task<GuestProfileResponse> GetGuestProfileByIdAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new GetGuestProfileByIdParams
            {
                Id = id,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, GetGuestProfileByIdParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Guest profile with ID {id} not found.");
            }

            return MapToResponse(result);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving guest profile with ID {Id}", id);
            throw;
        }
    }

    public async Task<GuestProfileResponse> GetGuestProfileByUserIdAsync(Guid userId)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new GetGuestProfileByUserIdParams
            {
                UserId = userId,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, GetGuestProfileByUserIdParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Guest profile not found for user {userId}.");
            }

            return MapToResponse(result);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving guest profile for user with ID {UserId}", userId);
            throw;
        }
    }

    public async Task<GuestProfileResponse> UpdateGuestProfileAsync(Guid id, UpdateGuestProfileRequest request)
    {
        try
        {
            // First check if the profile exists
            await GetGuestProfileByIdAsync(id);
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new UpdateGuestProfileParams
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality,
                PassportNumber = request.PassportNumber,
                PassportExpiryDate = request.PassportExpiryDate,
                PreferredLanguage = request.PreferredLanguage,
                NewsletterSubscribed = request.NewsletterSubscribed,
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, UpdateGuestProfileParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Guest profile with ID {id} not found.");
            }

            return MapToResponse(result);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating guest profile with ID {Id}", id);
            throw;
        }
    }

    public async Task<GuestProfileResponse> AddPreferenceAsync(Guid guestId, GuestPreferenceRequest preference)
    {
        try
        {
            // First check if the profile exists
            await GetGuestProfileByIdAsync(guestId);
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new AddGuestPreferenceParams
            {
                Id = Guid.NewGuid(),
                GuestId = guestId,
                PreferenceType = preference.Type.ToString(),
                Value = preference.Value,
                Notes = preference.Notes,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, AddGuestPreferenceParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new Exception("Failed to add preference to guest profile");
            }

            return MapToResponse(result);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding preference to guest with ID {GuestId}", guestId);
            throw;
        }
    }

    public async Task<bool> RemovePreferenceAsync(Guid guestId, Guid preferenceId)
    {
        try
        {
            // First check if the profile exists
            await GetGuestProfileByIdAsync(guestId);
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new RemoveGuestPreferenceParams
            {
                GuestId = guestId,
                PreferenceId = preferenceId,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, RemoveGuestPreferenceParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing preference {PreferenceId} from guest with ID {GuestId}", preferenceId, guestId);
            throw;
        }
    }

    public async Task<GuestProfileResponse> UpdateLoyaltyPointsAsync(Guid id, UpdateLoyaltyPointsRequest request)
    {
        try
        {
            // First get the current profile to calculate new loyalty tier
            var currentProfile = await GetGuestProfileByIdAsync(id);
            int newTotalPoints = currentProfile.LoyaltyPoints + request.PointsToAdd;
            LoyaltyTier newTier = CalculateLoyaltyTier(newTotalPoints);
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new UpdateLoyaltyPointsParams
            {
                Id = id,
                PointsToAdd = request.PointsToAdd,
                LoyaltyTier = newTier.ToString(),
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, UpdateLoyaltyPointsParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Guest profile with ID {id} not found.");
            }

            _logger.LogInformation(
                "Updated loyalty points for guest {GuestId}. Added {Points} points for {Reason}. New total: {Total}, New tier: {Tier}",
                id, request.PointsToAdd, request.Reason, result.LoyaltyPoints, result.LoyaltyTier);
                
            return MapToResponse(result);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating loyalty points for guest with ID {Id}", id);
            throw;
        }
    }

    public async Task<List<GuestProfileResponse>> SearchGuestsAsync(string searchTerm)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new SearchGuestsParams
            {
                SearchTerm = searchTerm,
                p_refcur_1 = null
            };

            var results = await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, SearchGuestsParams>(param, db);
            
            return results.Select(MapToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching guests with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<List<GuestProfileResponse>> GetGuestsByLoyaltyTierAsync(LoyaltyTier tier)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new GetGuestsByLoyaltyTierParams
            {
                LoyaltyTier = tier.ToString(),
                p_refcur_1 = null
            };

            var results = await _dapperRepo.ExecuteSpQueryAsync<GuestProfile, GetGuestsByLoyaltyTierParams>(param, db);
            
            return results.Select(MapToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving guests with loyalty tier {Tier}", tier);
            throw;
        }
    }

    private static GuestProfileResponse MapToResponse(GuestProfile profile)
    {
        return new GuestProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Address = profile.Address,
            City = profile.City,
            State = profile.State,
            Country = profile.Country,
            PostalCode = profile.PostalCode,
            DateOfBirth = profile.DateOfBirth,
            Nationality = profile.Nationality,
            PassportNumber = profile.PassportNumber,
            PassportExpiryDate = profile.PassportExpiryDate,
            PreferredLanguage = profile.PreferredLanguage,
            NewsletterSubscribed = profile.NewsletterSubscribed,
            Preferences = profile.Preferences.Select(p => new GuestPreferenceResponse
            {
                Id = p.Id,
                Type = p.Type,
                Value = p.Value,
                Notes = p.Notes
            }).ToList(),
            CreatedAt = profile.CreatedAt,
            ModifiedAt = profile.ModifiedAt,
            LoyaltyTier = profile.LoyaltyTier,
            LoyaltyPoints = profile.LoyaltyPoints
        };
    }

    private static LoyaltyTier CalculateLoyaltyTier(int points)
    {
        return points switch
        {
            >= 50000 => LoyaltyTier.Diamond,
            >= 25000 => LoyaltyTier.Platinum,
            >= 10000 => LoyaltyTier.Gold,
            >= 5000 => LoyaltyTier.Silver,
            _ => LoyaltyTier.Standard
        };
    }
}
