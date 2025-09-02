using DataAccess;

namespace HotelManagement.Services.Payment.SpInput;

public class GetPaymentByIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_payment_by_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
