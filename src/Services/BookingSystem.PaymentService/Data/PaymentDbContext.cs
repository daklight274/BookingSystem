using BookingSystem.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.PaymentService.Data
{
    public class PaymentDbContext:DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options):base(options)
        {
            
        }

        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Payment>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.Property(x => x.Status).HasConversion<string>();
                e.Property(x => x.Method).HasConversion<string>();
                e.HasIndex(x => x.BookingId);
            });
        }
    }
}
