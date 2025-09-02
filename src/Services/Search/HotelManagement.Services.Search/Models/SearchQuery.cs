namespace HotelManagement.Services.Search.Models;

public class SearchQuery
{
    public Guid Id { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public string? Type { get; set; }
    public DateTime RequestedAt { get; set; }
    public string? Status { get; set; }
    public string? ResultJson { get; set; }
    public string? Error { get; set; }
}
