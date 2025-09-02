using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pupdatedemandforecast")]
    public partial class UpdateDemandForecastParams : DbParameterHandler
    {
        [SqlParameter("@hotelid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

        [SqlParameter("@roomtypeid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid RoomTypeId { get; set; }

        [SqlParameter("@date")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime Date { get; set; }

        [SqlParameter("@expecteddemand")]
        [SqlDbType(System.Data.DbType.Int32)]
        public int ExpectedDemand { get; set; }

        [SqlParameter("@suggestedpriceadjustment")]
        [SqlDbType(System.Data.DbType.Decimal)]
        public decimal SuggestedPriceAdjustment { get; set; }

        [SqlParameter("@factors")]
        [SqlDbType(System.Data.DbType.String)]
        public string Factors { get; set; }

        [SqlParameter("@isactive")]
        [SqlDbType(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pupdatedemandforecast(
        in hotelid uuid,
        in roomtypeid uuid,
        in date timestamp,
        in expecteddemand int,
        in suggestedpriceadjustment decimal,
        in factors varchar,
        in isactive boolean,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    declare
        existingid uuid;
        newid uuid;
        currenttime timestamp;
    begin
        currenttime := now();
        select Id into existingid from DemandForecasts 
        where HotelId = hotelid and RoomTypeId = roomtypeid and Date = date;
        
        if existingid is null then
            -- Create new record
            newid := gen_random_uuid();
            insert into DemandForecasts (Id, HotelId, RoomTypeId, Date, ExpectedDemand, SuggestedPriceAdjustment, Factors, IsActive, CreatedAt)
            values (newid, hotelid, roomtypeid, date, expecteddemand, suggestedpriceadjustment, factors, isactive, currenttime);
            
            open p_refcur_1 for
            select * from DemandForecasts where Id = newid;
        else
            -- Update existing record
            update DemandForecasts
            set ExpectedDemand = expecteddemand,
                SuggestedPriceAdjustment = suggestedpriceadjustment,
                Factors = factors,
                IsActive = isactive,
                UpdatedAt = currenttime
            where Id = existingid;
            
            open p_refcur_1 for
            select * from DemandForecasts where Id = existingid;
        end if;
    end
    $procedure$;
    */
}
