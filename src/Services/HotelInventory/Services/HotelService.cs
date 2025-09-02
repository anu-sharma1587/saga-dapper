using DataAccess;
using HotelManagement.Services.HotelInventory.Models;
using HotelManagement.Services.HotelInventory.SpInput;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.HotelInventory.Services;

public class HotelService : IHotelService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<HotelService> _logger;

    public HotelService(IDataRepository dataRepository, ILogger<HotelService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<List<Hotel>> GetAllHotelsAsync(bool includeInactive = false)
    {
        try
        {
            var parameters = new GetAllHotelsParams
            {
                IncludeInactive = includeInactive
            };
            
            var result = await _dataRepository.QueryAsync<Hotel>(parameters);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hotels");
            throw;
        }
    }

    public async Task<Hotel> GetHotelByIdAsync(Guid id)
    {
        try
        {
            var parameters = new GetHotelByIdParams
            {
                Id = id
            };
            
            var hotel = await _dataRepository.QueryFirstOrDefaultAsync<Hotel>(parameters);
            
            if (hotel == null)
            {
                throw new KeyNotFoundException($"Hotel with ID {id} not found");
            }

            // Get address
            var addressParams = new GetHotelAddressParams { HotelId = id };
            var address = await _dataRepository.QueryFirstOrDefaultAsync<Address>(addressParams);
            if (address != null)
            {
                hotel.Address = address;
            }

            // Get contact info
            var contactsParams = new GetHotelContactsParams { HotelId = id };
            var contacts = await _dataRepository.QueryAsync<Contact>(contactsParams);
            if (contacts.Any())
            {
                hotel.ContactInfo = contacts.First(); // Assuming primary contact is first
            }

            // Get amenities
            var amenitiesParams = new GetHotelAmenitiesParams { HotelId = id };
            var amenities = await _dataRepository.QueryAsync<Amenity>(amenitiesParams);
            hotel.Amenities = amenities.ToList();

            // Get room types
            var roomTypesParams = new GetRoomTypesByHotelIdParams { HotelId = id };
            var roomTypes = await _dataRepository.QueryAsync<RoomType>(roomTypesParams);
            hotel.RoomTypes = roomTypes.ToList();

            // Get policies
            var policiesParams = new GetPoliciesByHotelIdParams { HotelId = id };
            var policies = await _dataRepository.QueryAsync<Policy>(policiesParams);
            hotel.Policies = policies.ToList();

            return hotel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hotel with ID {HotelId}", id);
            throw;
        }
    }

    public async Task<Hotel> CreateHotelAsync(Hotel hotel)
    {
        try
        {
            hotel.Id = hotel.Id != Guid.Empty ? hotel.Id : Guid.NewGuid();
            hotel.CreatedAt = DateTime.UtcNow;
            hotel.UpdatedAt = DateTime.UtcNow;
            hotel.IsActive = true;

            var parameters = new CreateHotelParams
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Category = hotel.Category,
                StarRating = hotel.StarRating,
                IsActive = hotel.IsActive,
                CreatedAt = hotel.CreatedAt,
                UpdatedAt = hotel.UpdatedAt
            };
            
            await _dataRepository.ExecuteAsync(parameters);
            
            // Add address if provided
            if (hotel.Address != null)
            {
                var addressParams = new UpdateHotelAddressParams
                {
                    HotelId = hotel.Id,
                    Street = hotel.Address.StreetAddress,
                    City = hotel.Address.City,
                    State = hotel.Address.State,
                    Country = hotel.Address.Country,
                    PostalCode = hotel.Address.PostalCode,
                    Latitude = hotel.Address.Latitude,
                    Longitude = hotel.Address.Longitude,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _dataRepository.ExecuteAsync(addressParams);
            }
            
            // Add contact info if provided
            if (hotel.ContactInfo != null)
            {
                var contactParams = new AddHotelContactParams
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotel.Id,
                    ContactType = "Email",
                    ContactValue = hotel.ContactInfo.Email,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _dataRepository.ExecuteAsync(contactParams);
                
                if (!string.IsNullOrEmpty(hotel.ContactInfo.Phone))
                {
                    var phoneParams = new AddHotelContactParams
                    {
                        Id = Guid.NewGuid(),
                        HotelId = hotel.Id,
                        ContactType = "Phone",
                        ContactValue = hotel.ContactInfo.Phone,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    await _dataRepository.ExecuteAsync(phoneParams);
                }
                
                if (!string.IsNullOrEmpty(hotel.ContactInfo.Website))
                {
                    var websiteParams = new AddHotelContactParams
                    {
                        Id = Guid.NewGuid(),
                        HotelId = hotel.Id,
                        ContactType = "Website",
                        ContactValue = hotel.ContactInfo.Website,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    await _dataRepository.ExecuteAsync(websiteParams);
                }
            }
            
            // Add amenities if provided
            if (hotel.Amenities != null && hotel.Amenities.Any())
            {
                foreach (var amenity in hotel.Amenities)
                {
                    var amenityParams = new AddHotelAmenityParams
                    {
                        HotelId = hotel.Id,
                        Name = amenity.Name,
                        Description = amenity.Description
                    };
                    
                    await _dataRepository.ExecuteAsync(amenityParams);
                }
            }
            
            return await GetHotelByIdAsync(hotel.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hotel {HotelName}", hotel.Name);
            throw;
        }
    }

    public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
    {
        try
        {
            hotel.UpdatedAt = DateTime.UtcNow;
            
            var parameters = new UpdateHotelParams
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Category = hotel.Category,
                StarRating = hotel.StarRating,
                IsActive = hotel.IsActive,
                UpdatedAt = hotel.UpdatedAt.Value
            };
            
            await _dataRepository.ExecuteAsync(parameters);
            
            // Update address if provided
            if (hotel.Address != null)
            {
                var addressParams = new UpdateHotelAddressParams
                {
                    HotelId = hotel.Id,
                    Street = hotel.Address.StreetAddress,
                    City = hotel.Address.City,
                    State = hotel.Address.State,
                    Country = hotel.Address.Country,
                    PostalCode = hotel.Address.PostalCode,
                    Latitude = hotel.Address.Latitude,
                    Longitude = hotel.Address.Longitude,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _dataRepository.ExecuteAsync(addressParams);
            }
            
            return await GetHotelByIdAsync(hotel.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hotel with ID {HotelId}", hotel.Id);
            throw;
        }
    }

    public async Task<bool> DeleteHotelAsync(Guid id)
    {
        try
        {
            var parameters = new DeleteHotelParams
            {
                Id = id
            };
            
            await _dataRepository.ExecuteAsync(parameters);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hotel with ID {HotelId}", id);
            throw;
        }
    }

    public async Task<List<Hotel>> SearchHotelsAsync(string searchTerm, string? category = null, int? starRating = null)
    {
        try
        {
            var parameters = new SearchHotelsParams
            {
                SearchTerm = searchTerm,
                Category = category,
                StarRating = starRating,
                IncludeInactive = false
            };
            
            var result = await _dataRepository.QueryAsync<Hotel>(parameters);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching hotels with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<RoomType> AddRoomTypeAsync(Guid hotelId, RoomType roomType)
    {
        try
        {
            roomType.Id = roomType.Id != Guid.Empty ? roomType.Id : Guid.NewGuid();
            roomType.HotelId = hotelId;
            roomType.IsActive = true;
            
            var parameters = new AddRoomTypeParams
            {
                Id = roomType.Id,
                HotelId = hotelId,
                Name = roomType.Name,
                Description = roomType.Description,
                MaxOccupancy = roomType.MaxOccupancy,
                TotalRooms = roomType.TotalRooms,
                BasePrice = roomType.BasePrice,
                SizeSqft = roomType.SizeSqft,
                BedConfiguration = roomType.BedConfiguration,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await _dataRepository.ExecuteAsync(parameters);
            
            // Add room amenities if provided
            if (roomType.RoomAmenities != null && roomType.RoomAmenities.Any())
            {
                foreach (var amenity in roomType.RoomAmenities)
                {
                    var amenityParams = new AddRoomAmenityParams
                    {
                        Id = Guid.NewGuid(),
                        RoomTypeId = roomType.Id,
                        Name = amenity.Name,
                        Description = amenity.Description,
                        IsHighlight = amenity.IsHighlight,
                        Icon = amenity.Icon
                    };
                    
                    await _dataRepository.ExecuteAsync(amenityParams);
                }
            }
            
            // Add room images if provided
            if (roomType.Images != null && roomType.Images.Any())
            {
                foreach (var image in roomType.Images)
                {
                    var imageParams = new AddRoomImageParams
                    {
                        Id = Guid.NewGuid(),
                        RoomTypeId = roomType.Id,
                        Url = image.Url,
                        Caption = image.Caption,
                        IsPrimary = image.IsPrimary,
                        DisplayOrder = image.DisplayOrder
                    };
                    
                    await _dataRepository.ExecuteAsync(imageParams);
                }
            }
            
            var result = await GetRoomTypeByIdAsync(roomType.Id);
            return result ?? throw new Exception($"Failed to retrieve room type with ID {roomType.Id} after creation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding room type to hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<bool> UpdateRoomInventoryAsync(Guid hotelId, Guid roomTypeId, int newTotalRooms)
    {
        try
        {
            var parameters = new UpdateRoomInventoryParams
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                NewTotalRooms = newTotalRooms,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _dataRepository.ExecuteAsync(parameters);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room inventory for hotel {HotelId}, room type {RoomTypeId}", hotelId, roomTypeId);
            throw;
        }
    }

    public async Task<List<RoomType>> GetRoomTypesByHotelIdAsync(Guid hotelId, bool includeInactive = false)
    {
        try
        {
            var parameters = new GetRoomTypesByHotelIdParams
            {
                HotelId = hotelId
            };
            
            var result = await _dataRepository.QueryAsync<RoomType>(parameters);
            
            // Filter inactive room types if required
            if (!includeInactive)
            {
                result = result.Where(rt => rt.IsActive).ToList();
            }
            
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room types for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<List<Amenity>> GetAmenitiesByHotelIdAsync(Guid hotelId, bool includeInactive = false)
    {
        try
        {
            var parameters = new GetHotelAmenitiesParams
            {
                HotelId = hotelId
            };
            
            var result = await _dataRepository.QueryAsync<Amenity>(parameters);
            
            // Filter inactive amenities if required
            if (!includeInactive)
            {
                result = result.Where(a => a.IsActive).ToList();
            }
            
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving amenities for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<List<Policy>> GetPoliciesByHotelIdAsync(Guid hotelId, bool includeInactive = false)
    {
        try
        {
            var parameters = new GetPoliciesByHotelIdParams
            {
                HotelId = hotelId
            };
            
            var result = await _dataRepository.QueryAsync<Policy>(parameters);
            
            // Filter inactive policies if required
            if (!includeInactive)
            {
                result = result.Where(p => p.IsActive).ToList();
            }
            
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policies for hotel {HotelId}", hotelId);
            throw;
        }
    }

    private async Task<RoomType?> GetRoomTypeByIdAsync(Guid id)
    {
        try
        {
            var parameters = new GetRoomTypeByIdParams
            {
                Id = id
            };
            
            return await _dataRepository.QueryFirstOrDefaultAsync<RoomType>(parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room type with ID {RoomTypeId}", id);
            throw;
        }
    }
}
