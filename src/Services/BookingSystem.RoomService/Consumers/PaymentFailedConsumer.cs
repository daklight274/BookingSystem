using BookingSystem.RoomService.Data;
using BookingSystem.Shared.Contracts;
using MassTransit;

namespace BookingSystem.RoomService.Consumers
{
    public class PaymentFailedConsumer
    : IConsumer<PaymentFailedEvent>
    {
        private readonly RoomDbContext _db;
        private readonly ILogger _logger;

        public PaymentFailedConsumer(
            RoomDbContext db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentFailedEvent> context)
        {
            var msg = context.Message;
            var room = await _db.Rooms.FindAsync(msg.RoomId);

            if (room is null) return;

            // Compensating transaction: release phòng
            room.IsAvailable = true;
            await _db.SaveChangesAsync();

            _logger.LogWarning(
                "Room {RoomId} → released sau khi payment failed: {Reason}",
                msg.RoomId, msg.Reason);
        }
    }
}
