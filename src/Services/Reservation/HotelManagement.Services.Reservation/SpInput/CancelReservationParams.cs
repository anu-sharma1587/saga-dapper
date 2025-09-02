using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class CancelReservationParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_cancel_reservation";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string CancellationReason { get; set; } = null!;
    public DateTime CancelledAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
