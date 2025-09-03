using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
        [DbProcedureAttribute("pgetactivepricingrules")]
    public partial class GetActivePricingRulesParams : DbParameterHandler
    {
    [SqlParameterAttribute("@hotelid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

    [SqlParameterAttribute("@roomtypeid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid? RoomTypeId { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
}
