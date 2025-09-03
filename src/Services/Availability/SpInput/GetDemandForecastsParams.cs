using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
        [DbProcedureAttribute("pgetdemandforecasts")]
    public partial class GetDemandForecastsParams : DbParameterHandler
    {
    [SqlParameterAttribute("@hotelid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

    [SqlParameterAttribute("@roomtypeid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid RoomTypeId { get; set; }

    [SqlParameterAttribute("@startdate")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime StartDate { get; set; }

    [SqlParameterAttribute("@enddate")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime EndDate { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pgetdemandforecasts(
        in hotelid uuid,
        in roomtypeid uuid,
        in startdate timestamp,
        in enddate timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        open p_refcur_1 for
        select * from DemandForecasts
        where HotelId = hotelid 
        and RoomTypeId = roomtypeid 
        and Date >= startdate
        and Date <= enddate
        order by Date;
    end
    $procedure$;
    */
}
