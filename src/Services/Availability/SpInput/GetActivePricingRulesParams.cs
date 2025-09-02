using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pgetactivepricingrules")]
    public partial class GetActivePricingRulesParams : DbParameterHandler
    {
        [SqlParameter("@hotelid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

        [SqlParameter("@roomtypeid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid? RoomTypeId { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
}
