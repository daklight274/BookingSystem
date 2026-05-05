using BookingSystem.BookingService.Data;
using BookingSystem.BookingService.Models;
using BookingSystem.Shared.Contracts;
using MassTransit;

namespace BookingSystem.BookingService.Consumers
{
    public class PaymentFailedConsumer
    : IConsumer<PaymentFailedEvent>
    {
        private readonly BookingDbContext _db;
        private readonly ILogger _logger;

        public PaymentFailedConsumer(
            BookingDbContext db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentFailedEvent> context)
        {
            var msg = context.Message;
            var booking = await _db.Bookings.FindAsync(msg.BookingId);

            if (booking is null) return;

            // Compensating transaction: huỷ booking
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogWarning(
                "Booking {BookingId} → CANCELLED vì thanh toán thất bại: {Reason}",
                msg.BookingId, msg.Reason);

            // Room Service cũng sẽ nhận PaymentFailedEvent
            // và tự release phòng — không cần Booking Service gọi lại
        }
    }
}
