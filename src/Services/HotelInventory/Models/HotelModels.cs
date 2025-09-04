using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.HotelInventory.Models;

public class Hotel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., Luxury, Business, Resort
    public int StarRating { get; set; }
    public Address Address { get; set; } = null!;
    public Contact ContactInfo { get; set; } = null!;
    public List<Amenity> Amenities { get; set; } = new();
    public List<RoomType> RoomTypes { get; set; } = new();
    public List<Policy> Policies { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class Address
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Hotel? Hotel { get; set; }
}

public class Contact
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? FacebookPage { get; set; }
    public string? TwitterHandle { get; set; }
    public string? InstagramHandle { get; set; }
    public Hotel? Hotel { get; set; }
}

public class Amenity
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., General, Room, Property
    public bool IsChargeable { get; set; }
    public decimal? Cost { get; set; }
    public string? CostUnit { get; set; } // e.g., per day, per use
    public bool IsActive { get; set; }
    public Hotel? Hotel { get; set; }
}

public class RoomType
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxOccupancy { get; set; }
    public int TotalRooms { get; set; }
    public decimal BasePrice { get; set; }
    public int SizeSqft { get; set; }
    public string BedConfiguration { get; set; } = string.Empty; // e.g., 1 King, 2 Queen
    public List<RoomAmenity> RoomAmenities { get; set; } = new();
    public List<Image> Images { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Hotel? Hotel { get; set; }
}

public class RoomAmenity
{
    public Guid Id { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsHighlight { get; set; }
    public RoomType? RoomType { get; set; }
}

public class Image
{
    public Guid Id { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public RoomType? RoomType { get; set; }
}

public class Policy
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Terms { get; set; }
    public bool IsActive { get; set; }
    public Hotel? Hotel { get; set; }
}
