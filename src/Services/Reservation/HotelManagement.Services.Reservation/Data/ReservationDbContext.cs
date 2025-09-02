using Microsoft.EntityFrameworkCore;
using HotelManagement.Services.Reservation.Models;

namespace HotelManagement.Services.Reservation.Data;

public class ReservationDbContext : DbContext
{
    public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.HotelId).IsRequired();
            entity.Property(e => e.RoomTypeId).IsRequired();
            entity.Property(e => e.GuestId).IsRequired();
            entity.Property(e => e.CheckInDate).IsRequired();
            entity.Property(e => e.CheckOutDate).IsRequired();
            entity.Property(e => e.NumberOfRooms).IsRequired();
            entity.Property(e => e.NumberOfGuests).IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.DepositAmount).HasColumnType("decimal(18,2)").IsRequired();
            
            // Indexing for common queries
            entity.HasIndex(e => e.GuestId);
            entity.HasIndex(e => e.HotelId);
            entity.HasIndex(e => new { e.CheckInDate, e.CheckOutDate });
            entity.HasIndex(e => e.Status);
        });
    }
}
