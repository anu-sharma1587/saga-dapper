using Microsoft.EntityFrameworkCore;
using HotelManagement.Services.Reporting.Models;

namespace HotelManagement.Services.Reporting.Data;

public class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options) { }
    public DbSet<ReportJob> ReportJobs => Set<ReportJob>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReportJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
    }
}
