using HotelManagement.Services.Notifications.DTOs;
using HotelManagement.Services.Notifications.Models;
using HotelManagement.Services.Notifications.SpInput;
using DataAccess;
using System.Data.Common;

namespace HotelManagement.Services.Notifications.Services;


public class NotificationService : INotificationService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IDataRepository dataRepository, ILogger<NotificationService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<NotificationResponse> SendNotificationAsync(SendNotificationRequest request)
    {
        try
        {
            var parameters = new SendNotificationParams
            {
                Id = Guid.NewGuid(),
                RecipientId = request.RecipientId,
                RecipientType = request.RecipientType,
                Type = request.Type,
                Subject = request.Subject,
                Message = request.Message,
                SentAt = DateTime.UtcNow,
                Status = "Sent"
            };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<Notification, SendNotificationParams>(parameters);
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            throw;
        }
    }

    public async Task<NotificationResponse?> GetNotificationByIdAsync(Guid id)
    {
        try
        {
            var parameters = new GetNotificationByIdParams { Id = id };
            var result = await _dataRepository.QueryFirstOrDefaultStoredProcedureAsync<Notification, GetNotificationByIdParams>(parameters);
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification by id {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<NotificationResponse>> GetNotificationsByRecipientAsync(Guid recipientId)
    {
        try
        {
            var parameters = new GetNotificationsByRecipientParams { RecipientId = recipientId };
            var results = await _dataRepository.QueryStoredProcedureAsync<Notification, GetNotificationsByRecipientParams>(parameters);
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications by recipient {RecipientId}", recipientId);
            return Enumerable.Empty<NotificationResponse>();
        }
    }

    public async Task<bool> CompensateCancelNotificationAsync(Guid id)
    {
        try
        {
            var parameters = new CompensateCancelNotificationParams { Id = id, Status = "Cancelled" };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<CompensateCancelNotificationParams>(parameters);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling notification {Id}", id);
            return false;
        }
    }

    private static NotificationResponse MapToResponse(Notification n) => new()
    {
        Id = n.Id,
        RecipientId = n.RecipientId,
        RecipientType = n.RecipientType,
        Type = n.Type,
        Subject = n.Subject,
        Message = n.Message,
        SentAt = n.SentAt,
        Status = n.Status.ToString(),
        Error = n.Error
    };
}
