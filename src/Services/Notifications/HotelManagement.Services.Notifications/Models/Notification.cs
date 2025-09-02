namespace HotelManagement.Services.Notifications.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientType { get; set; } = null!; // Guest, Staff, etc.
    public string Type { get; set; } = null!; // Email, SMS, Push
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public NotificationStatus Status { get; set; }
    public string? Error { get; set; }
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Cancelled
}
