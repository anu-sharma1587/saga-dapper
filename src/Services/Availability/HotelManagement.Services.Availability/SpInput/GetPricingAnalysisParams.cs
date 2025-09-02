using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pgetpricinganalysis")]
    public partial class GetPricingAnalysisParams : DbParameterHandler
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

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pgetpricinganalysis(
        in hotelid uuid,
        in roomtypeid uuid,
        in date timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        open p_refcur_1 for
        select * from RoomAvailabilities
        where HotelId = hotelid 
        and RoomTypeId = roomtypeid 
        and Date = date;
    end
    $procedure$;
    */
}
