using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;
namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedureAttribute("pupdateavailability")]
    public partial class UpdateAvailabilityParams : DbParameterHandler
    {
    [SqlParameterAttribute("@hotelid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

    [SqlParameterAttribute("@roomtypeid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid RoomTypeId { get; set; }

    [SqlParameterAttribute("@date")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime Date { get; set; }

    [SqlParameterAttribute("@availablerooms")]
    [SqlDbTypeAttribute(System.Data.DbType.Int32)]
        public int AvailableRooms { get; set; }

    [SqlParameterAttribute("@baseprice")]
    [SqlDbTypeAttribute(System.Data.DbType.Decimal)]
        public decimal? BasePrice { get; set; }

    [SqlParameterAttribute("@currentprice")]
    [SqlDbTypeAttribute(System.Data.DbType.Decimal)]
        public decimal CurrentPrice { get; set; }

    [SqlParameterAttribute("@lastupdated")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime LastUpdated { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pupdateavailability(
        in hotelid uuid,
        in roomtypeid uuid,
        in date timestamp,
        in availablerooms int,
        in baseprice decimal,
        in currentprice decimal,
        in lastupdated timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    declare
        existingid uuid;
        newid uuid;
    begin
        select Id into existingid from RoomAvailabilities 
        where HotelId = hotelid and RoomTypeId = roomtypeid and Date = date;
        
        if existingid is null then
            -- Create new record
            newid := gen_random_uuid();
            insert into RoomAvailabilities (Id, HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, BasePrice, CurrentPrice, LastUpdated)
            values (newid, hotelid, roomtypeid, date, availablerooms, availablerooms, baseprice, currentprice, lastupdated);
            
            open p_refcur_1 for
            select * from RoomAvailabilities where Id = newid;
        else
            -- Update existing record
            update RoomAvailabilities
            set AvailableRooms = availablerooms,
                BasePrice = coalesce(baseprice, BasePrice),
                CurrentPrice = currentprice,
                LastUpdated = lastupdated
            where Id = existingid;
            
            open p_refcur_1 for
            select * from RoomAvailabilities where Id = existingid;
        end if;
    end
    $procedure$;
    */
}
