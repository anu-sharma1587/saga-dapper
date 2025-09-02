using DataAccess;

namespace HotelManagement.Services.Reporting.SpInput;

public class CancelReportParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_cancel_report";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
