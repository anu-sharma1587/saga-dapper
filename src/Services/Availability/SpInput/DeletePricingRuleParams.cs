using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
        [DbProcedureAttribute("pdeletepricingrule")]
    public partial class DeletePricingRuleParams : DbParameterHandler
    {
    [SqlParameterAttribute("@id")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
}
