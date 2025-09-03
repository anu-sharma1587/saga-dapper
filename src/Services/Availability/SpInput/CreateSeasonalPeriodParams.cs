using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;
namespace HotelManagement.Services.Availability.SpInput
{
        [DbProcedureAttribute("pcreateseasonalperiod")]
    public partial class CreateSeasonalPeriodParams : DbParameterHandler
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

    [SqlParameterAttribute("@baseadjustmentpercentage")]
    [SqlDbTypeAttribute(System.Data.DbType.Decimal)]
        public decimal BaseAdjustmentPercentage { get; set; }

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
    /*
    Example stored procedure:
    create or replace procedure pcreateseasonalperiod(
        in hotelid uuid,
        in name varchar,
        in description varchar,
        in startdate timestamp,
        in enddate timestamp,
        in baseadjustmentpercentage decimal,
        in isactive boolean,
        in createdat timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    declare
        newid uuid;
    begin
        newid := gen_random_uuid();
        insert into SeasonalPeriods (Id, HotelId, Name, Description, StartDate, EndDate, BaseAdjustmentPercentage, IsActive, CreatedAt)
        values (newid, hotelid, name, description, startdate, enddate, baseadjustmentpercentage, isactive, createdat);
        
        open p_refcur_1 for
        select * from SeasonalPeriods where Id = newid;
    end
    $procedure$;
    */
}
