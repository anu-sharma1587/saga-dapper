using DataAccess;

namespace HotelManagement.Services.Notifications.SpInput;

public class SendNotificationParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientType { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public string Status { get; set; } = "Sent";
    public string? Error { get; set; }

    public string StoredProcedureName => "sp_SendNotification";
}
