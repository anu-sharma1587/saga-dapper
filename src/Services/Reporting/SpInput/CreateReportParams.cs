using DataAccess;

namespace HotelManagement.Services.Reporting.SpInput;

public class CreateReportParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_create_report";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
}
