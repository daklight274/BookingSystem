using BookingSystem.PaymentService.Data;
using BookingSystem.PaymentService.Dtos;
using BookingSystem.PaymentService.Models;
using BookingSystem.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.PaymentService.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            PaymentDbContext db,
            IPublishEndpoint publisher,
            ILogger<PaymentController> logger)
        {
            _db = db;
            _publisher = publisher;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var p = await _db.Payments.FindAsync(id);
            return p is null ? NotFound() : Ok(ToResponse(p));
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetByBooking(string bookingId)
        {
            var payments = await _db.Payments
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return Ok(payments.Select(ToResponse));
        }

        // ── Xử lý thanh toán ─────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Process(ProcessPaymentRequest req)
        {
            // Kiểm tra booking đã có payment chưa
            var existing = await _db.Payments
                .FirstOrDefaultAsync(p =>
                    p.BookingId == req.BookingId &&
                    p.Status == PaymentStatus.Completed);

            if (existing != null)
                return BadRequest(new { error = "Booking này đã được thanh toán rồi." });

            var payment = new Payment
            {
                BookingId = req.BookingId,
                RoomId = req.RoomId,
                Amount = req.Amount,
                Method = req.Method,
                Notes = req.Notes,
                Status = PaymentStatus.Pending
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            // ── Mock xử lý thanh toán ─────────────────────────
            // Thực tế: gọi Stripe/VNPay/MoMo API ở đây
            // Mock: số lẻ thì fail, số chẵn thì success
            var isSuccess = req.Amount % 2 == 0;

            if (isSuccess)
            {
                payment.Status = PaymentStatus.Completed;
                payment.PaidAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                // Publish thành công → Booking Service cập nhật Confirmed
                await _publisher.Publish(new PaymentCompletedEvent
                {
                    PaymentId = payment.Id,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    Method = payment.Method.ToString(),
                    PaidAt = payment.PaidAt.Value
                });

                _logger.LogInformation(
                    "Payment {Id} COMPLETED cho booking {BookingId}",
                    payment.Id, payment.BookingId);
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                // Publish thất bại → Booking Service + Room Service rollback
                await _publisher.Publish(new PaymentFailedEvent
                {
                    BookingId = payment.BookingId,
                    RoomId = payment.RoomId,
                    Reason = "Thanh toán thất bại — số tiền không hợp lệ (mock)"
                });

                _logger.LogWarning(
                    "Payment {Id} FAILED cho booking {BookingId}",
                    payment.Id, payment.BookingId);
            }

            return CreatedAtAction(nameof(GetById),
                new { id = payment.Id }, ToResponse(payment));
        }

        // ── Hoàn tiền ─────────────────────────────────────────
        [HttpPost("{id}/refund")]
        public async Task<IActionResult> Refund(string id)
        { 
            var payment = await _db.Payments.FindAsync(id);
            if (payment is null) return NotFound();

            if (payment.Status != PaymentStatus.Completed)
                return BadRequest(new
                {
                    error = $"Chỉ hoàn tiền được payment Completed. Hiện tại: {payment.Status}"
                });

            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Payment {Id} đã hoàn tiền", id);
            return Ok(ToResponse(payment));
        }

        private static PaymentResponse ToResponse(Payment p) => new(
            p.Id, p.BookingId, p.Amount,
            p.Status.ToString(), p.Method.ToString(),
            p.CreatedAt, p.PaidAt);
    }
}
