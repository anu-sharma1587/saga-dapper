using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pdeletepricingrule")]
    public partial class DeletePricingRuleParams : DbParameterHandler
    {
        [SqlParameter("@id")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
}
