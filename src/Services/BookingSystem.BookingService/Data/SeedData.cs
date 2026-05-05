using BookingSystem.BookingService.Models;

namespace BookingSystem.BookingService.Data
{
    public static class SeedData
    {
        public static void Initialize(BookingDbContext db)
        {
            if (db.Bookings.Any()) return;

            // Seed vài booking mẫu để test
            db.Bookings.AddRange(
                new Booking
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomId = Guid.NewGuid().ToString(), // thay bằng RoomId thật nếu muốn
                    GuestName = "Nguyen Van A",
                    GuestEmail = "a@example.com",
                    GuestPhone = "0901234567",
                    CheckIn = DateTime.UtcNow.AddDays(1),
                    CheckOut = DateTime.UtcNow.AddDays(3),
                    TotalPrice = 1_800_000,
                    Status = BookingStatus.Confirmed,
                    Notes = "Yêu cầu phòng tầng cao"
                },
                new Booking
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomId = Guid.NewGuid().ToString(),
                    GuestName = "Tran Thi B",
                    GuestEmail = "b@example.com",
                    GuestPhone = "0912345678",
                    CheckIn = DateTime.UtcNow.AddDays(5),
                    CheckOut = DateTime.UtcNow.AddDays(7),
                    TotalPrice = 5_000_000,
                    Status = BookingStatus.Pending,
                    Notes = null
                }
            );

            db.SaveChanges();
        }
    }
}
