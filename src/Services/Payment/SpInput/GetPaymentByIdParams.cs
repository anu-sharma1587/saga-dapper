using DataAccess.Dapper;

namespace HotelManagement.Services.Payment.SpInput;

public class GetPaymentByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
}
