using BookingSystem.BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.BookingService.Data
{
    public class BookingDbContext:DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options):base(options) { }

        public DbSet<Booking> Bookings => Set<Booking>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.GuestName).IsRequired().HasMaxLength(200);
                e.Property(x => x.GuestEmail).HasMaxLength(200);
                e.Property(x => x.TotalPrice).HasPrecision(18, 2);
                e.Property(x => x.Status).HasConversion<string>();
                e.Ignore(x => x.NightCount);
            });
        }
    }
}
