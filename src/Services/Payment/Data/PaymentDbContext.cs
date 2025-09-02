using Microsoft.EntityFrameworkCore;
using HotelManagement.Services.Payment.Models;

namespace HotelManagement.Services.Payment.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefundRecord> Refunds => Set<RefundRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentProvider).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);
        });
        modelBuilder.Entity<RefundRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.TransactionId).HasMaxLength(100);
        });
    }
}
