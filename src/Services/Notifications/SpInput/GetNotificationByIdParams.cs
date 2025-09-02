using DataAccess;

namespace HotelManagement.Services.Notifications.SpInput;

public class GetNotificationByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }

    public string StoredProcedureName => "sp_GetNotificationById";
}
