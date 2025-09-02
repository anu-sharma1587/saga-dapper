using DataAccess;

namespace HotelManagement.Services.Notifications.SpInput;

public class CompensateCancelNotificationParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Cancelled";

    public string StoredProcedureName => "sp_CancelNotification";
}
