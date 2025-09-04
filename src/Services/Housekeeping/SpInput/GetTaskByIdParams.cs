using DataAccess.Dapper;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class GetTaskByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
}
