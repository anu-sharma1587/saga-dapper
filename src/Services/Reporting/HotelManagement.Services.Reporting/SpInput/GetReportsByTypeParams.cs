using DataAccess;

namespace HotelManagement.Services.Reporting.SpInput;

public class GetReportsByTypeParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_reports_by_type";
    public object? p_refcur_1 { get; set; }
    
    public string Type { get; set; } = null!;
}
