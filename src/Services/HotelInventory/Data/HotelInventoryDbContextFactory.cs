using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelManagement.Services.HotelInventory.Data;

public class HotelInventoryDbContextFactory : IDesignTimeDbContextFactory<HotelInventoryDbContext>
{
    public HotelInventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HotelInventoryDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=hotelmanagement_hotelinventory;Username=postgres;Password=postgres");

        return new HotelInventoryDbContext(optionsBuilder.Options);
    }
}
