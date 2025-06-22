using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Data
{
    public class UWCRugbyDbContext : DbContext
    {
        public UWCRugbyDbContext(DbContextOptions<UWCRugbyDbContext> options) : base(options) { }

        public DbSet<PlayerGpsData> GpsData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerGpsData>().ToTable("gps_data", "uwc_mens_15s").HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
