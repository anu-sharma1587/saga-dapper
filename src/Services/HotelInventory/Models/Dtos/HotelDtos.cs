using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.HotelInventory.Models.Dtos;

public record CreateHotelRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Required]
    public string Category { get; init; } = string.Empty;

    [Range(1, 7)]
    public int StarRating { get; init; }

    [Required]
    public AddressDto Address { get; init; } = null!;

    [Required]
    public ContactDto ContactInfo { get; init; } = null!;

    public List<CreateAmenityRequest>? Amenities { get; init; }
    public List<CreateRoomTypeRequest>? RoomTypes { get; init; }
    public List<CreatePolicyRequest>? Policies { get; init; }
}

public record UpdateHotelRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Required]
    public string Category { get; init; } = string.Empty;

    [Range(1, 7)]
    public int StarRating { get; init; }

    public bool IsActive { get; init; }
}

public record AddressDto
{
    [Required]
    public string StreetAddress { get; init; } = string.Empty;

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    public string State { get; init; } = string.Empty;

    [Required]
    public string Country { get; init; } = string.Empty;

    [Required]
    public string PostalCode { get; init; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; init; }

    [Range(-180, 180)]
    public double Longitude { get; init; }
}

public record ContactDto
{
    [Required]
    [Phone]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Url]
    public string? Website { get; init; }

    public string? FacebookPage { get; init; }
    public string? TwitterHandle { get; init; }
    public string? InstagramHandle { get; init; }
}

public record CreateAmenityRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Required]
    public string Category { get; init; } = string.Empty;

    public bool IsChargeable { get; init; }
    public decimal? Cost { get; init; }
    public string? CostUnit { get; init; }
}

public record CreateRoomTypeRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Range(1, 10)]
    public int MaxOccupancy { get; init; }

    [Range(0, 1000)]
    public int TotalRooms { get; init; }

    [Range(0, 100000)]
    public decimal BasePrice { get; init; }

    [Range(0, 10000)]
    public int SizeSqft { get; init; }

    [Required]
    public string BedConfiguration { get; init; } = string.Empty;

    public List<CreateRoomAmenityRequest>? RoomAmenities { get; init; }
    public List<CreateImageRequest>? Images { get; init; }
}

public record CreateRoomAmenityRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    public string? Icon { get; init; }
    public bool IsHighlight { get; init; }
}

public record CreateImageRequest
{
    [Required]
    [Url]
    public string Url { get; init; } = string.Empty;

    [Required]
    public string Caption { get; init; } = string.Empty;

    public bool IsPrimary { get; init; }
    public int DisplayOrder { get; init; }
}

public record CreatePolicyRequest
{
    [Required]
    public string Type { get; init; } = string.Empty;

    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    public string? Terms { get; init; }
}

public record HotelResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int StarRating { get; init; }
    public Address Address { get; init; } = null!;
    public Contact ContactInfo { get; init; } = null!;
    public List<Amenity> Amenities { get; init; } = new();
    public List<RoomType> RoomTypes { get; init; } = new();
    public List<Policy> Policies { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}
