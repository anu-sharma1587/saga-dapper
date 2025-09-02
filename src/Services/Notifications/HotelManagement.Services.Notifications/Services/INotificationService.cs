using HotelManagement.Services.Notifications.DTOs;

namespace HotelManagement.Services.Notifications.Services;

public interface INotificationService
{
    Task<NotificationResponse> SendNotificationAsync(SendNotificationRequest request);
    Task<NotificationResponse?> GetNotificationByIdAsync(Guid id);
    Task<IEnumerable<NotificationResponse>> GetNotificationsByRecipientAsync(Guid recipientId);
    Task<bool> CompensateCancelNotificationAsync(Guid id);
}
