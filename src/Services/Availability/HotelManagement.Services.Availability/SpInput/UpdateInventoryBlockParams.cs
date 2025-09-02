using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pupdateinventoryblock")]
    public partial class UpdateInventoryBlockParams : DbParameterHandler
    {
        [SqlParameter("@id")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

        [SqlParameter("@hotelid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

        [SqlParameter("@roomtypeid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid RoomTypeId { get; set; }

        [SqlParameter("@startdate")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime StartDate { get; set; }

        [SqlParameter("@enddate")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime EndDate { get; set; }

        [SqlParameter("@blockedrooms")]
        [SqlDbType(System.Data.DbType.Int32)]
        public int BlockedRooms { get; set; }

        [SqlParameter("@reason")]
        [SqlDbType(System.Data.DbType.String)]
        public string Reason { get; set; }

        [SqlParameter("@reference")]
        [SqlDbType(System.Data.DbType.String)]
        public string Reference { get; set; }

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
    /*
    Example stored procedure:
    create or replace procedure pupdateinventoryblock(
        in id uuid,
        in hotelid uuid,
        in roomtypeid uuid,
        in startdate timestamp,
        in enddate timestamp,
        in blockedrooms int,
        in reason varchar,
        in reference varchar,
        in isactive boolean,
        in updatedat timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        update InventoryBlocks 
        set StartDate = startdate, 
            EndDate = enddate, 
            BlockedRooms = blockedrooms, 
            Reason = reason, 
            Reference = reference, 
            IsActive = isactive, 
            UpdatedAt = updatedat
        where Id = id;
        
        open p_refcur_1 for
        select * from InventoryBlocks where Id = id;
    end
    $procedure$;
    */
}
