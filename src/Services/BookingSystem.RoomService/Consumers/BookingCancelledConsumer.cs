using BookingSystem.RoomService.Data;
using BookingSystem.Shared.Contracts;
using MassTransit;

namespace BookingSystem.RoomService.Consumers
{
    public class BookingCancelledConsumer : IConsumer<BookingCancelledEvent>
    {
        private readonly RoomDbContext _db;
        private readonly ILogger _logger;

        public BookingCancelledConsumer(
            RoomDbContext db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingCancelledEvent> context)
        {
            var msg = context.Message;
            var room = await _db.Rooms.FindAsync(msg.RoomId);

            if (room is null) return;

            // Release phòng — compensating transaction
            room.IsAvailable = true;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Đã release phòng {RoomNumber} sau khi huỷ booking {BookingId}",
                room.RoomNumber, msg.BookingId);
        }
    }
}
