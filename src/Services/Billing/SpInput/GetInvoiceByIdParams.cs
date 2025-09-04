using DataAccess.Dapper;

namespace HotelManagement.Services.Billing.SpInput;

public class GetInvoiceByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
}
