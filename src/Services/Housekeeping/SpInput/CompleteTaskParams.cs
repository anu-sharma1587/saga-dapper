using DataAccess.Dapper;

namespace HotelManagement.Services.Housekeeping.SpInput;

public class CompleteTaskParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Status { get; set; } = "Completed";
    public string? Notes { get; set; }
}
