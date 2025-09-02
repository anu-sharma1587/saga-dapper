using HotelManagement.Services.HotelInventory.Models;

namespace HotelManagement.Services.HotelInventory.Services;

public interface IHotelService
{
    Task<List<Hotel>> GetAllHotelsAsync(bool includeInactive = false);
    Task<Hotel> GetHotelByIdAsync(Guid id);
    Task<Hotel> CreateHotelAsync(Hotel hotel);
    Task<Hotel> UpdateHotelAsync(Hotel hotel);
    Task<bool> DeleteHotelAsync(Guid id);
    Task<List<Hotel>> SearchHotelsAsync(string searchTerm, string? category = null, int? starRating = null);
    Task<RoomType> AddRoomTypeAsync(Guid hotelId, RoomType roomType);
    Task<bool> UpdateRoomInventoryAsync(Guid hotelId, Guid roomTypeId, int newTotalRooms);
    Task<List<RoomType>> GetRoomTypesByHotelIdAsync(Guid hotelId, bool includeInactive = false);
    Task<List<Amenity>> GetAmenitiesByHotelIdAsync(Guid hotelId, bool includeInactive = false);
    Task<List<Policy>> GetPoliciesByHotelIdAsync(Guid hotelId, bool includeInactive = false);
}
