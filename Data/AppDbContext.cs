using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapApplication.Data {
    public class AppDbContext : DbContext {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration) {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Point> Points { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Point>().HasData(
                    new Point { Id = 1, Wkt = "POLYGON((10.689 -25.092, 34.595 -20.170, 38.814 -35.639, 13.502 -39.155, 10.689 -25.092))", Name = "AreaTest" },
                    new Point { Id = 2, Wkt = "POINT((34, 34))", Name = "PointTest" }
                );
        }
    }
}
