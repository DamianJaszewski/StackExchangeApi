using Microsoft.EntityFrameworkCore;
using StackExchangeApi.Models;

namespace StackExchangeApi
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<ExternalLink> ExternalLinks { get; set; }
        public DbSet<Collective> Collectives { get; set; }
    }
}
