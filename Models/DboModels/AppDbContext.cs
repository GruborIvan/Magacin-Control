using CSS_MagacinControl_App.Modules;
using Microsoft.EntityFrameworkCore;

namespace CSS_MagacinControl_App.Models.DboModels
{
    public class AppDbContext : DbContext
    {
        public DbSet<IdentDbo> RobaZaPakovanjeItem { get; set; }
        public DbSet<FakturaDbo> RobaZaPakovanje { get; set; }
        public DbSet<IdentBarkodDbo> IdentBarkod { get; set; }
        public DbSet<UserDbo> Users { get; set; }

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = DbConnectionModule.GetConnectionString();
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}