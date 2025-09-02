using DataAccess;

namespace HotelManagement.Services.Notifications.SpInput;

public class GetNotificationsByRecipientParams : IStoredProcedureParams
{
    public Guid RecipientId { get; set; }

    public string StoredProcedureName => "sp_GetNotificationsByRecipient";
}
