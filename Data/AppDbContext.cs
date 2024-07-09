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
    }
}
