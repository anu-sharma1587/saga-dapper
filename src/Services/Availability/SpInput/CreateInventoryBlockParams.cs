using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedureAttribute("pcreateinventoryblock")]
    public partial class CreateInventoryBlockParams : DbParameterHandler
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

        [SqlParameterAttribute("@blockedrooms")]
        [SqlDbTypeAttribute(System.Data.DbType.Int32)]
        public int BlockedRooms { get; set; }

        [SqlParameterAttribute("@reason")]
        [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string Reason { get; set; }

        [SqlParameterAttribute("@reference")]
        [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string Reference { get; set; }

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
    create or replace procedure pcreateinventoryblock(
        in hotelid uuid,
        in roomtypeid uuid,
        in startdate timestamp,
        in enddate timestamp,
        in blockedrooms int,
        in reason varchar,
        in reference varchar,
        in isactive boolean,
        in createdat timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    declare
        newid uuid;
    begin
        newid := gen_random_uuid();
        insert into InventoryBlocks (Id, HotelId, RoomTypeId, StartDate, EndDate, BlockedRooms, Reason, Reference, IsActive, CreatedAt)
        values (newid, hotelid, roomtypeid, startdate, enddate, blockedrooms, reason, reference, isactive, createdat);
        
        open p_refcur_1 for
        select * from InventoryBlocks where Id = newid;
    end
    $procedure$;
    */
}
