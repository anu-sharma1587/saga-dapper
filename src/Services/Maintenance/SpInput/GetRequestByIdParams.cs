using DataAccess.Dapper;

namespace HotelManagement.Services.Maintenance.SpInput;

public class GetRequestByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
}
