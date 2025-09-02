using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Search.SpInput
{
    [DbProcedure("pgetsearchesbytype")]
    public partial class GetSearchesByTypeParams : DbParameterHandler
    {
        [SqlParameter("@type")]
        [SqlDbType(System.Data.DbType.String)]
        public string Type { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pgetsearchesbytype(
        in type varchar,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        open p_refcur_1 for
        select * from SearchQueries where Type = type;
    end
    $procedure$;
    */
}
