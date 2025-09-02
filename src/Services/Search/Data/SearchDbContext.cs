using HotelManagement.Services.Search.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services.Search.Data;

public class SearchDbContext : DbContext
{
    public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options) { }

    public DbSet<SearchQuery> SearchQueries => Set<SearchQuery>();
}
