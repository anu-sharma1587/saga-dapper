using DataAccess.Dapper;

namespace HotelManagement.Services.Payment.SpInput;

public class GetPaymentsByReservationIdParams : IStoredProcedureParams
{
    public Guid ReservationId { get; set; }
}
