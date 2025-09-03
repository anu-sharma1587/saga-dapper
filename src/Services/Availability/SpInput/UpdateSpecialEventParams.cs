using System;
using DataAccess.Mediator.Attributes;
using DataAccess.Mediator.Handler;
namespace HotelManagement.Services.Availability.SpInput
{
        [DbProcedureAttribute("pupdatespecialevent")]
    public partial class UpdateSpecialEventParams : DbParameterHandler
    {
    [SqlParameterAttribute("@id")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid Id { get; set; }

    [SqlParameterAttribute("@hotelid")]
    [SqlDbTypeAttribute(System.Data.DbType.Guid)]
        public Guid HotelId { get; set; }

    [SqlParameterAttribute("@name")]
    [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string Name { get; set; }

    [SqlParameterAttribute("@description")]
    [SqlDbTypeAttribute(System.Data.DbType.String)]
        public string Description { get; set; }

    [SqlParameterAttribute("@startdate")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime StartDate { get; set; }

    [SqlParameterAttribute("@enddate")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime EndDate { get; set; }

    [SqlParameterAttribute("@impactpercentage")]
    [SqlDbTypeAttribute(System.Data.DbType.Decimal)]
        public decimal ImpactPercentage { get; set; }

    [SqlParameterAttribute("@expecteddemandincrease")]
    [SqlDbTypeAttribute(System.Data.DbType.Int32)]
        public int ExpectedDemandIncrease { get; set; }

    [SqlParameterAttribute("@isactive")]
    [SqlDbTypeAttribute(System.Data.DbType.Boolean)]
        public bool IsActive { get; set; }

    [SqlParameterAttribute("@updatedat")]
    [SqlDbTypeAttribute(System.Data.DbType.DateTime)]
        public DateTime UpdatedAt { get; set; }

    [SqlParameterAttribute("p_refcur_1")]
    [PostgresRefCursorAttribute("p_refcur_1")]
        public object p_refcur_1 { get; set; }
    }
    /*
    Example stored procedure:
    create or replace procedure pupdatespecialevent(
        in id uuid,
        in hotelid uuid,
        in name varchar,
        in description varchar,
        in startdate timestamp,
        in enddate timestamp,
        in impactpercentage decimal,
        in expecteddemandincrease int,
        in isactive boolean,
        in updatedat timestamp,
        InOut p_refcur_1 refcursor default 'p_refcur_1'::refcursor)
    language plpgsql
    as $procedure$
    begin
        update SpecialEvents 
        set Name = name,
            Description = description,
            StartDate = startdate,
            EndDate = enddate,
            ImpactPercentage = impactpercentage,
            ExpectedDemandIncrease = expecteddemandincrease,
            IsActive = isactive,
            UpdatedAt = updatedat
        where Id = id;
        
        open p_refcur_1 for
        select * from SpecialEvents where Id = id;
    end
    $procedure$;
    */
}
