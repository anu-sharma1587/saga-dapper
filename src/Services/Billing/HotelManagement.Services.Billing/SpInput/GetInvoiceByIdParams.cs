using DataAccess;

namespace HotelManagement.Services.Billing.SpInput;

public class GetInvoiceByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }

    public string StoredProcedureName => "sp_GetInvoiceById";
}
