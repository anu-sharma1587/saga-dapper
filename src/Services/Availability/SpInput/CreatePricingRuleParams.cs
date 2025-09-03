using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;
namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedureAttribute("pcreatepricingrule")]
    public partial class CreatePricingRuleParams : DbParameterHandler
    {
    [SqlParameterAttribute("@hotelid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

    [SqlParameterAttribute("@name")]
    [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string Name { get; set; }

    [SqlParameterAttribute("@description")]
    [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string Description { get; set; }

    [SqlParameterAttribute("@startdate")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime StartDate { get; set; }

    [SqlParameterAttribute("@enddate")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime EndDate { get; set; }

    [SqlParameterAttribute("@daysofweek")]
    [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string DaysOfWeek { get; set; }

    [SqlParameterAttribute("@adjustmentpercentage")]
    [SqlDbTypeAttribute(System.Data.DbType.Decimal)]
        public decimal AdjustmentPercentage { get; set; }

    [SqlParameterAttribute("@priority")]
    [SqlDbTypeAttribute(System.Data.DbType.Int32)]
        public int Priority { get; set; }

    [SqlParameterAttribute("@isactive")]
    [SqlDbTypeAttribute(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

    [SqlParameterAttribute("@createdat")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime CreatedAt { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
}
