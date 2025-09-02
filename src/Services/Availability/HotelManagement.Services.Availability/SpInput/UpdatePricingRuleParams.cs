using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pupdatepricingrule")]
    public partial class UpdatePricingRuleParams : DbParameterHandler
    {
        [SqlParameter("@id")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

        [SqlParameter("@hotelid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

        [SqlParameter("@name")]
        [SqlDbType(System.Data.DbType.String)]
        public string Name { get; set; }

        [SqlParameter("@description")]
        [SqlDbType(System.Data.DbType.String)]
        public string Description { get; set; }

        [SqlParameter("@startdate")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime StartDate { get; set; }

        [SqlParameter("@enddate")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime EndDate { get; set; }

        [SqlParameter("@daysofweek")]
        [SqlDbType(System.Data.DbType.String)]
        public string DaysOfWeek { get; set; }

        [SqlParameter("@adjustmentpercentage")]
        [SqlDbType(System.Data.DbType.Decimal)]
        public decimal AdjustmentPercentage { get; set; }

        [SqlParameter("@priority")]
        [SqlDbType(System.Data.DbType.Int32)]
        public int Priority { get; set; }

        [SqlParameter("@isactive")]
        [SqlDbType(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

        [SqlParameter("@updatedat")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
}
