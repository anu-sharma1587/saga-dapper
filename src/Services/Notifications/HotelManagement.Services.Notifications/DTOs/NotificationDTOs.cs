namespace HotelManagement.Services.Notifications.DTOs;

public record SendNotificationRequest(
    Guid RecipientId,
    string RecipientType,
    string Type,
    string Subject,
    string Message
);

public record NotificationResponse
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientType { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public string Status { get; set; } = null!;
    public string? Error { get; set; }
}
