using HotelManagement.Services.HotelInventory.Models;
using HotelManagement.Services.HotelInventory.Models.Dtos;
using HotelManagement.Services.HotelInventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.HotelInventory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILogger<HotelsController> _logger;

    public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<HotelResponse>>> GetHotels([FromQuery] bool includeInactive = false)
    {
        try
        {
            var hotels = await _hotelService.GetAllHotelsAsync(includeInactive);
            var response = hotels.Select(h => MapToResponse(h)).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hotels");
            return StatusCode(500, new { message = "An error occurred while retrieving hotels" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HotelResponse>> GetHotelById(Guid id)
    {
        try
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            return Ok(MapToResponse(hotel));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hotel with ID {HotelId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the hotel" });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<HotelResponse>> CreateHotel([FromBody] CreateHotelRequest request)
    {
        try
        {
            var hotel = new Hotel
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                StarRating = request.StarRating,
                Address = new Address
                {
                    StreetAddress = request.Address.StreetAddress,
                    City = request.Address.City,
                    State = request.Address.State,
                    Country = request.Address.Country,
                    PostalCode = request.Address.PostalCode,
                    Latitude = request.Address.Latitude,
                    Longitude = request.Address.Longitude
                },
                ContactInfo = new Contact
                {
                    Phone = request.ContactInfo.Phone,
                    Email = request.ContactInfo.Email,
                    Website = request.ContactInfo.Website,
                    FacebookPage = request.ContactInfo.FacebookPage,
                    TwitterHandle = request.ContactInfo.TwitterHandle,
                    InstagramHandle = request.ContactInfo.InstagramHandle
                }
            };

            if (request.Amenities != null)
            {
                hotel.Amenities = request.Amenities.Select(a => new Amenity
                {
                    Name = a.Name,
                    Description = a.Description,
                    Category = a.Category,
                    IsChargeable = a.IsChargeable,
                    Cost = a.Cost,
                    CostUnit = a.CostUnit,
                    IsActive = true
                }).ToList();
            }

            if (request.RoomTypes != null)
            {
                hotel.RoomTypes = request.RoomTypes.Select(r => new RoomType
                {
                    Name = r.Name,
                    Description = r.Description,
                    MaxOccupancy = r.MaxOccupancy,
                    TotalRooms = r.TotalRooms,
                    BasePrice = r.BasePrice,
                    SizeSqft = r.SizeSqft,
                    BedConfiguration = r.BedConfiguration,
                    IsActive = true,
                    RoomAmenities = r.RoomAmenities?.Select(ra => new RoomAmenity
                    {
                        Name = ra.Name,
                        Description = ra.Description,
                        Icon = ra.Icon,
                        IsHighlight = ra.IsHighlight
                    }).ToList() ?? new List<RoomAmenity>(),
                    Images = r.Images?.Select(i => new Image
                    {
                        Url = i.Url,
                        Caption = i.Caption,
                        IsPrimary = i.IsPrimary,
                        DisplayOrder = i.DisplayOrder
                    }).ToList() ?? new List<Image>()
                }).ToList();
            }

            if (request.Policies != null)
            {
                hotel.Policies = request.Policies.Select(p => new Policy
                {
                    Name = p.Name,
                    Description = p.Description,
                    Terms = p.Terms,
                    IsActive = true
                }).ToList();
            }

            var createdHotel = await _hotelService.CreateHotelAsync(hotel);
            return CreatedAtAction(nameof(GetHotelById), new { id = createdHotel.Id }, MapToResponse(createdHotel));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hotel {HotelName}", request.Name);
            return StatusCode(500, new { message = "An error occurred while creating the hotel" });
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<HotelResponse>> UpdateHotel(Guid id, [FromBody] UpdateHotelRequest request)
    {
        try
        {
            var existingHotel = await _hotelService.GetHotelByIdAsync(id);
            existingHotel.Name = request.Name;
            existingHotel.Description = request.Description;
            existingHotel.Category = request.Category;
            existingHotel.StarRating = request.StarRating;
            existingHotel.IsActive = request.IsActive;

            var updatedHotel = await _hotelService.UpdateHotelAsync(existingHotel);
            return Ok(MapToResponse(updatedHotel));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hotel with ID {HotelId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the hotel" });
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        try
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Hotel with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hotel with ID {HotelId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the hotel" });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<HotelResponse>>> SearchHotels(
        [FromQuery] string searchTerm,
        [FromQuery] string? category = null,
        [FromQuery] int? starRating = null)
    {
        try
        {
            var hotels = await _hotelService.SearchHotelsAsync(searchTerm, category, starRating);
            var response = hotels.Select(h => MapToResponse(h)).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching hotels with term {SearchTerm}", searchTerm);
            return StatusCode(500, new { message = "An error occurred while searching hotels" });
        }
    }

    [HttpGet("{id:guid}/rooms")]
    public async Task<ActionResult<List<RoomType>>> GetHotelRoomTypes(Guid id, [FromQuery] bool includeInactive = false)
    {
        try
        {
            var roomTypes = await _hotelService.GetRoomTypesByHotelIdAsync(id, includeInactive);
            return Ok(roomTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room types for hotel {HotelId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving room types" });
        }
    }

    private static HotelResponse MapToResponse(Hotel hotel)
    {
        return new HotelResponse
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Category = hotel.Category,
            StarRating = hotel.StarRating,
            Address = hotel.Address,
            ContactInfo = hotel.ContactInfo,
            Amenities = hotel.Amenities,
            RoomTypes = hotel.RoomTypes,
            Policies = hotel.Policies,
            CreatedAt = hotel.CreatedAt,
            UpdatedAt = hotel.UpdatedAt,
            IsActive = hotel.IsActive
        };
    }
}
