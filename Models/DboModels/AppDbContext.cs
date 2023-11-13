using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

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
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("MagacinDbConnectionStringLocal");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}