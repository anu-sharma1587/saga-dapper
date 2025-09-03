using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;
namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pcreateseasonalperiod")]
    public partial class CreateSeasonalPeriodParams : DbParameterHandler
    {
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

        [SqlParameter("@baseadjustmentpercentage")]
        [SqlDbType(System.Data.DbType.Decimal)]
        public decimal BaseAdjustmentPercentage { get; set; }

        [SqlParameter("@isactive")]
        [SqlDbType(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

        [SqlParameter("@createdat")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
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
