using BookingSystem.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.RoomService.Data
{
    public class RoomDbContext : DbContext
    {
        public RoomDbContext(DbContextOptions<RoomDbContext>options):base(options) { }

        public DbSet<Room> Rooms => Set<Room>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Room>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.RoomNumber).IsRequired().HasMaxLength(10);
                e.Property(x => x.PricePerNight).HasPrecision(18, 2);
                e.Property(x => x.Type).HasConversion<string>();
            });
        }
    }
}
