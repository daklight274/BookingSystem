using BookingSystem.RoomService.Models;

namespace BookingSystem.RoomService.Data
{
    public static class SeedData
    {
        public static void Initialize(RoomDbContext db)
        {
            if (db.Rooms.Any()) return; // đã có data rồi thì bỏ qua

            db.Rooms.AddRange(
                new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomNumber = "101",
                    Type = RoomType.Single,
                    PricePerNight = 500_000,
                    MaxGuests = 1,
                    Description = "Phòng đơn tầng 1 view sân vườn"
                },
                new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomNumber = "201",
                    Type = RoomType.Double,
                    PricePerNight = 900_000,
                    MaxGuests = 2,
                    Description = "Phòng đôi tầng 2 view hồ bơi"
                },
                new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomNumber = "301",
                    Type = RoomType.Suite,
                    PricePerNight = 2_500_000,
                    MaxGuests = 4,
                    Description = "Suite tầng 3 ban công riêng"
                },
                new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomNumber = "202",
                    Type = RoomType.Double,
                    PricePerNight = 850_000,
                    MaxGuests = 2,
                    Description = "Phòng đôi tầng 2 view thành phố"
                },
                new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    RoomNumber = "102",
                    Type = RoomType.Single,
                    PricePerNight = 450_000,
                    MaxGuests = 1,
                    Description = "Phòng đơn tầng 1 tiêu chuẩn"
                }
            );
            db.SaveChanges();
        }
    }
}
