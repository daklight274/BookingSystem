using BookingSystem.BookingService.Data;
using BookingSystem.BookingService.Models;
using BookingSystem.Shared.Contracts;
using MassTransit;

namespace BookingSystem.BookingService.Consumers
{
    public class PaymentCompletedConsumer
    : IConsumer<PaymentCompletedEvent>
    {
        private readonly BookingDbContext _db;
        private readonly ILogger _logger;

        public PaymentCompletedConsumer(
            BookingDbContext db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentCompletedEvent> context)
        {
            var msg = context.Message;
            var booking = await _db.Bookings.FindAsync(msg.BookingId);

            if (booking is null)
            {
                _logger.LogWarning(
                    "Không tìm thấy booking {BookingId}", msg.BookingId);
                return;
            }

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Booking {BookingId} → CONFIRMED sau thanh toán {PaymentId}",
                msg.BookingId, msg.PaymentId);
        }
    }
}
