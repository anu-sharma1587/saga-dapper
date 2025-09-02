using DataAccess;

namespace HotelManagement.Services.Reservation.SpInput;

public class GetReservationsByHotelIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_reservations_by_hotel_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid HotelId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
