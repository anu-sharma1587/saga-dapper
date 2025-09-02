using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Search.SpInput
{
    [DbProcedure("pcreatesearchquery")]
    public partial class CreateSearchQueryParams : DbParameterHandler
    {
        [SqlParameter("@querytext")]
        [SqlDbType(System.Data.DbType.String)]
        public string QueryText { get; set; }

        [SqlParameter("@type")]
        [SqlDbType(System.Data.DbType.String)]
        public string Type { get; set; }

        [SqlParameter("@requestedat")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime RequestedAt { get; set; }

        [SqlParameter("@status")]
        [SqlDbType(System.Data.DbType.String)]
        public string Status { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pcreatesearchquery(
        in querytext varchar,
        in type varchar,
        in requestedat timestamp,
        in status varchar,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    declare
        newid uuid;
    begin
        newid := gen_random_uuid();
        insert into SearchQueries (Id, QueryText, Type, RequestedAt, Status)
        values (newid, querytext, type, requestedat, status);
        
        open p_refcur_1 for
        select * from SearchQueries where Id = newid;
    end
    $procedure$;
    */
}
