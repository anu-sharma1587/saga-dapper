using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pgetactiveinventoryblocks")]
    public partial class GetActiveInventoryBlocksParams : DbParameterHandler
    {
        [SqlParameter("@hotelid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

        [SqlParameter("@roomtypeid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid? RoomTypeId { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pgetactiveinventoryblocks(
        in hotelid uuid,
        in roomtypeid uuid,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        open p_refcur_1 for
        select * from InventoryBlocks 
        where HotelId = hotelid 
        and IsActive = true
        and (roomtypeid is null or RoomTypeId = roomtypeid);
    end
    $procedure$;
    */
}
