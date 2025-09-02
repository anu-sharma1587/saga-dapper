using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class UpdateReservationStatusParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_reservation_status";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public string? CancellationReason { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}
