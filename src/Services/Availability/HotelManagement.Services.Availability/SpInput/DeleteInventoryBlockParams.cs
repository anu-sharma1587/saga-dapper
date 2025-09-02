using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pdeleteinventoryblock")]
    public partial class DeleteInventoryBlockParams : DbParameterHandler
    {
        [SqlParameter("@id")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pdeleteinventoryblock(
        in id uuid,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        update InventoryBlocks 
        set IsActive = false,
            UpdatedAt = now()
        where Id = id;
        
        open p_refcur_1 for
        select * from InventoryBlocks where Id = id;
    end
    $procedure$;
    */
}
