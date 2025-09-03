using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
        [DbProcedureAttribute("pgetroomavailability")]
    public partial class GetRoomAvailabilityParams : DbParameterHandler
    {
    [SqlParameterAttribute("@hotelid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

    [SqlParameterAttribute("@checkin")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime CheckIn { get; set; }

    [SqlParameterAttribute("@checkout")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime CheckOut { get; set; }

    [SqlParameterAttribute("@roomtypeid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid? RoomTypeId { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pgetroomavailability(
        in hotelid uuid,
        in checkin timestamp,
        in checkout timestamp,
        in roomtypeid uuid,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
      open p_refcur_1 for
         select * from RoomAvailabilities where HotelId = hotelid and Date >= checkin and Date < checkout and (roomtypeid is null or RoomTypeId = roomtypeid);
    end
    $procedure$;
    */
}
