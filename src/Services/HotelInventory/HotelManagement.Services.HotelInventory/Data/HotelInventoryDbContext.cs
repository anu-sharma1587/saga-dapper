using HotelManagement.Services.HotelInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services.HotelInventory.Data;

public class HotelInventoryDbContext : DbContext
{
    public HotelInventoryDbContext(DbContextOptions<HotelInventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<Amenity> Amenities { get; set; } = null!;
    public DbSet<RoomType> RoomTypes { get; set; } = null!;
    public DbSet<RoomAmenity> RoomAmenities { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<Policy> Policies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.StarRating).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.Address)
                .WithOne(a => a.Hotel)
                .HasForeignKey<Address>(a => a.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ContactInfo)
                .WithOne(c => c.Hotel)
                .HasForeignKey<Contact>(c => c.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Amenities)
                .WithOne(a => a.Hotel)
                .HasForeignKey(a => a.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.RoomTypes)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Policies)
                .WithOne(p => p.Hotel)
                .HasForeignKey(p => p.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StreetAddress).IsRequired();
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Phone).IsRequired();
            entity.Property(e => e.Email).IsRequired();
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.BasePrice).HasPrecision(10, 2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasMany(e => e.RoomAmenities)
                .WithOne(a => a.RoomType)
                .HasForeignKey(a => a.RoomTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Images)
                .WithOne(i => i.RoomType)
                .HasForeignKey(i => i.RoomTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RoomAmenity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Caption).IsRequired();
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });
    }
}
