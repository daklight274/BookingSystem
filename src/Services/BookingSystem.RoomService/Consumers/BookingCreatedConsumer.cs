using BookingSystem.RoomService.Data;
using BookingSystem.Shared.Contracts;
using MassTransit;

namespace BookingSystem.RoomService.Consumers
{
    public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
    {
        private readonly RoomDbContext _db;
        private readonly ILogger _logger;

        public BookingCreatedConsumer(
            RoomDbContext db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation(
                "Nhận BookingCreated: BookingId={BookingId}, RoomId={RoomId}",
                msg.BookingId, msg.RoomId);

            var room = await _db.Rooms.FindAsync(msg.RoomId);

            if (room is null)
            {
                _logger.LogWarning("Không tìm thấy phòng {RoomId}", msg.RoomId);
                return;
            }

            if (!room.IsAvailable)
            {
                _logger.LogWarning(
                    "Phòng {RoomId} đã bị lock trước đó", msg.RoomId);
                return;
            }

            // Lock phòng lại
            room.IsAvailable = false;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Đã lock phòng {RoomNumber} cho booking {BookingId}",
                room.RoomNumber, msg.BookingId);
        }
    }
}
