using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedureAttribute("pupdateseasonalperiod")]
    public partial class UpdateSeasonalPeriodParams : DbParameterHandler
    {
    [SqlParameterAttribute("@id")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

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

    [SqlParameterAttribute("@baseadjustmentpercentage")]
    [SqlDbTypeAttribute(System.Data.DbType.Decimal)]
        public decimal BaseAdjustmentPercentage { get; set; }

    [SqlParameterAttribute("@isactive")]
    [SqlDbTypeAttribute(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

    [SqlParameterAttribute("@updatedat")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime UpdatedAt { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pupdateseasonalperiod(
        in id uuid,
        in hotelid uuid,
        in name varchar,
        in description varchar,
        in startdate timestamp,
        in enddate timestamp,
        in baseadjustmentpercentage decimal,
        in isactive boolean,
        in updatedat timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        update SeasonalPeriods 
        set Name = name,
            Description = description,
            StartDate = startdate,
            EndDate = enddate,
            BaseAdjustmentPercentage = baseadjustmentpercentage,
            IsActive = isactive,
            UpdatedAt = updatedat
        where Id = id;
        
        open p_refcur_1 for
        select * from SeasonalPeriods where Id = id;
    end
    $procedure$;
    */
}
