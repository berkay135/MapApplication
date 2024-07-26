﻿using Microsoft.AspNetCore.Identity;
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
        public DbSet<Polygon> Polygons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Point>().HasData(
                    new Point { Id = 1, X = 123, Y = 456, Name = "FromDbAnkara" },
                    new Point { Id = 2, X = 345, Y = 567, Name = "Bursa" }
                );
            modelBuilder.Entity<Polygon>().HasData(
                    new Polygon { Id = 1, Wkt = "POLYGON((10.689 -25.092, 34.595 -20.170, 38.814 -35.639, 13.502 -39.155, 10.689 -25.092))", Name = "AreaTest"  }
                );
        }
    }
}
