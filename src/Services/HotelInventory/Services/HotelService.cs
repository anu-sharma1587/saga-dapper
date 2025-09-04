using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using HotelManagement.Services.HotelInventory.Models;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.HotelInventory.Services;

public class HotelService : IHotelService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<HotelService> _logger;

    public HotelService(IDapperDataRepository dataRepository, IDbConnectionFactory connectionFactory, ILogger<HotelService> logger)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<List<Hotel>> GetAllHotelsAsync(bool includeInactive = false)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetAllHotelsAsync not fully implemented with current interface");
            return new List<Hotel>();
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
            using var connection = await _connectionFactory.CreateAsync();
            
            var hotel = await _dataRepository.FindByIDAsync<Hotel>(id, connection);
            
            if (hotel == null)
            {
                throw new KeyNotFoundException($"Hotel with ID {id} not found");
            }

            // Note: The following queries would need custom methods or we'd need to implement more specific query methods
            // For now, creating mock data since the current interface doesn't support these specific queries
            _logger.LogWarning("GetHotelByIdAsync using mock data for related entities - needs proper database query methods");
            
            // Mock address
            hotel.Address = new Address
            {
                Id = Guid.NewGuid(),
                HotelId = id,
                StreetAddress = "Mock Street",
                City = "Mock City",
                State = "Mock State",
                Country = "Mock Country",
                PostalCode = "12345"
            };

            // Mock contact info
            hotel.ContactInfo = new Contact
            {
                Id = Guid.NewGuid(),
                HotelId = id,
                Phone = "+1-555-0123",
                Email = "mock@hotel.com"
            };

            // Mock amenities
            hotel.Amenities = new List<Amenity>
            {
                new Amenity { Id = Guid.NewGuid(), HotelId = id, Name = "WiFi", Description = "Free WiFi" },
                new Amenity { Id = Guid.NewGuid(), HotelId = id, Name = "Pool", Description = "Swimming Pool" }
            };

            // Mock room types
            hotel.RoomTypes = new List<RoomType>
            {
                new RoomType { Id = Guid.NewGuid(), HotelId = id, Name = "Standard", Description = "Standard Room" },
                new RoomType { Id = Guid.NewGuid(), HotelId = id, Name = "Deluxe", Description = "Deluxe Room" }
            };

            // Mock policies
            hotel.Policies = new List<Policy>
            {
                new Policy { Id = Guid.NewGuid(), HotelId = id, Name = "Check-in", Description = "3:00 PM Check-in" },
                new Policy { Id = Guid.NewGuid(), HotelId = id, Name = "Check-out", Description = "11:00 AM Check-out" }
            };

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
            using var connection = await _connectionFactory.CreateAsync();
            
            hotel.Id = hotel.Id != Guid.Empty ? hotel.Id : Guid.NewGuid();
            hotel.CreatedAt = DateTime.UtcNow;
            hotel.UpdatedAt = DateTime.UtcNow;

            await _dataRepository.AddAsync(hotel, connection);
            return hotel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hotel");
            throw;
        }
    }

    public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            hotel.UpdatedAt = DateTime.UtcNow;
            await _dataRepository.UpdateAsync(hotel, hotel.Id, connection);
            return hotel;
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
            using var connection = await _connectionFactory.CreateAsync();
            
            var hotel = await _dataRepository.FindByIDAsync<Hotel>(id, connection);
            if (hotel == null)
            {
                return false;
            }

            hotel.IsActive = false;
            hotel.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dataRepository.UpdateAsync(hotel, hotel.Id, connection);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hotel with ID {HotelId}", id);
            return false;
        }
    }

    public async Task<List<Hotel>> SearchHotelsAsync(string searchTerm, string? category = null, int? starRating = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("SearchHotelsAsync not fully implemented with current interface");
            return new List<Hotel>();
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
            using var connection = await _connectionFactory.CreateAsync();
            
            roomType.Id = roomType.Id != Guid.Empty ? roomType.Id : Guid.NewGuid();
            roomType.HotelId = hotelId;
            roomType.CreatedAt = DateTime.UtcNow;
            roomType.UpdatedAt = DateTime.UtcNow;

            await _dataRepository.AddAsync(roomType, connection);
            return roomType;
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
            using var connection = await _connectionFactory.CreateAsync();
            
            var roomType = await _dataRepository.FindByIDAsync<RoomType>(roomTypeId, connection);
            if (roomType == null)
            {
                return false;
            }

            roomType.TotalRooms = newTotalRooms;
            roomType.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dataRepository.UpdateAsync(roomType, roomType.Id, connection);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room inventory for hotel {HotelId}, room type {RoomTypeId}", hotelId, roomTypeId);
            return false;
        }
    }

    public async Task<List<RoomType>> GetRoomTypesByHotelIdAsync(Guid hotelId, bool includeInactive = false)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetRoomTypesByHotelIdAsync not fully implemented with current interface");
            return new List<RoomType>();
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
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetAmenitiesByHotelIdAsync not fully implemented with current interface");
            return new List<Amenity>();
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
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetPoliciesByHotelIdAsync not fully implemented with current interface");
            return new List<Policy>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policies for hotel {HotelId}", hotelId);
            throw;
        }
    }
}
