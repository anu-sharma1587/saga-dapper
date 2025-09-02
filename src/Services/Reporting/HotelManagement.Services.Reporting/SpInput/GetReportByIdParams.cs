using DataAccess;

namespace HotelManagement.Services.Reporting.SpInput;

public class GetReportByIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_report_by_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
