using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;

namespace HotelManagement.Services.Availability.SpInput
{
    [DbProcedure("pcreatespecialevent")]
    public partial class CreateSpecialEventParams : DbParameterHandler
    {
        [SqlParameter("@hotelid")]
        [SqlDbType(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

        [SqlParameter("@name")]
        [SqlDbType(System.Data.DbType.String)]
        public string Name { get; set; }

        [SqlParameter("@description")]
        [SqlDbType(System.Data.DbType.String)]
        public string Description { get; set; }

        [SqlParameter("@startdate")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime StartDate { get; set; }

        [SqlParameter("@enddate")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime EndDate { get; set; }

        [SqlParameter("@impactpercentage")]
        [SqlDbType(System.Data.DbType.Decimal)]
        public decimal ImpactPercentage { get; set; }

        [SqlParameter("@expecteddemandincrease")]
        [SqlDbType(System.Data.DbType.Int32)]
        public int ExpectedDemandIncrease { get; set; }

        [SqlParameter("@isactive")]
        [SqlDbType(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

        [SqlParameter("@createdat")]
        [SqlDbType(System.Data.DbType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [SqlParameter("p_refcur_1")]
        [PostgresRefCursor("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pcreatespecialevent(
        in hotelid uuid,
        in name varchar,
        in description varchar,
        in startdate timestamp,
        in enddate timestamp,
        in impactpercentage decimal,
        in expecteddemandincrease int,
        in isactive boolean,
        in createdat timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    declare
        newid uuid;
    begin
        newid := gen_random_uuid();
        insert into SpecialEvents (Id, HotelId, Name, Description, StartDate, EndDate, ImpactPercentage, ExpectedDemandIncrease, IsActive, CreatedAt)
        values (newid, hotelid, name, description, startdate, enddate, impactpercentage, expecteddemandincrease, isactive, createdat);
        
        open p_refcur_1 for
        select * from SpecialEvents where Id = newid;
    end
    $procedure$;
    */
}
