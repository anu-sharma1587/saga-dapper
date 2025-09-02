using DataAccess;

namespace HotelManagement.Services.Payment.SpInput;

public class GetPaymentsByReservationIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_payments_by_reservation_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid ReservationId { get; set; }
}
