namespace HotelManagement.Services.Reporting.Models;

public class ReportJob
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ResultUrl { get; set; }
    public string? Error { get; set; }
}
