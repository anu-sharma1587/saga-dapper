using Microsoft.EntityFrameworkCore;
using HotelManagement.Services.Guest.Models;

namespace HotelManagement.Services.Guest.Data;

public class GuestDbContext : DbContext
{
    public GuestDbContext(DbContextOptions<GuestDbContext> options)
        : base(options)
    {
    }

    public DbSet<GuestProfile> GuestProfiles { get; set; }
    public DbSet<GuestPreference> GuestPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuestProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Nationality).HasMaxLength(100);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.Property(e => e.PreferredLanguage).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // Indexing for common queries
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => new { e.FirstName, e.LastName });
            entity.HasIndex(e => e.LoyaltyTier);
        });

        modelBuilder.Entity<GuestPreference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.GuestId).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => new { e.GuestId, e.Type });
        });
    }
}
