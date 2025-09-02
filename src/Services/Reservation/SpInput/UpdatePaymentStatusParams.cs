using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class UpdatePaymentStatusParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_payment_status";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public bool IsPaid { get; set; }
    public decimal? DepositAmount { get; set; }
    public DateTime ModifiedAt { get; set; }
}
