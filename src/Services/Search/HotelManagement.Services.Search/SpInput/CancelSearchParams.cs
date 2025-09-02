using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Search.SpInput
{
    [DbProcedure("pcancelsearch")]
    public partial class CancelSearchParams : DbParameterHandler
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
    create or replace procedure pcancelsearch(
        in id uuid,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        update SearchQueries
        set Status = 'Cancelled'
        where Id = id;
        
        open p_refcur_1 for
        select * from SearchQueries where Id = id;
    end
    $procedure$;
    */
}
