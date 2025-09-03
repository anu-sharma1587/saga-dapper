using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedureAttribute("pdeletespecialevent")]
    public partial class DeleteSpecialEventParams : DbParameterHandler
    {
        [SqlParameterAttribute("@id")]
        [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

        [SqlParameterAttribute("p_refcur_1")]
        [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pdeletespecialevent(
        in id uuid,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        update SpecialEvents
        set IsActive = false,
            UpdatedAt = now()
        where Id = id;
        
        open p_refcur_1 for
        select * from SpecialEvents where Id = id;
    end
    $procedure$;
    */
}
