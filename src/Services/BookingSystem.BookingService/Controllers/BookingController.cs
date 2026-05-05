using BookingSystem.BookingService.Data;
using BookingSystem.BookingService.Dtos;
using BookingSystem.BookingService.Models;
using BookingSystem.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.BookingService.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingDbContext _db;
        private readonly IPublishEndpoint _publisher;  // MassTransit

        public BookingController(
            BookingDbContext db,
            IPublishEndpoint publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status)
        {
            var q = _db.Bookings.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<BookingStatus>(status, true, out var s))
                q = q.Where(b => b.Status == s);

            var list = await q
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => ToResponse(b))
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var b = await _db.Bookings.FindAsync(id);
            return b is null ? NotFound() : Ok(ToResponse(b));
        }

        // ── Tạo booking + publish event ─────────────
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingRequest req)
        {
            // Validate ngày
            if (req.CheckIn >= req.CheckOut)
                return BadRequest(new { error = "CheckOut phải sau CheckIn" });

            if (req.CheckIn.Date < DateTime.UtcNow.Date)
                return BadRequest(new { error = "CheckIn không thể là ngày trong quá khứ" });

            var nights = (int)(req.CheckOut - req.CheckIn).TotalDays;
            var totalPrice = nights * req.PricePerNight;

            var booking = new Booking
            {
                RoomId = req.RoomId,
                GuestName = req.GuestName,
                GuestEmail = req.GuestEmail,
                GuestPhone = req.GuestPhone,
                CheckIn = req.CheckIn,
                CheckOut = req.CheckOut,
                TotalPrice = totalPrice,
                Notes = req.Notes,
                Status = BookingStatus.Pending
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            // ── Publish event → RabbitMQ ─────────────
            // Room Service sẽ nhận event này và lock phòng
            await _publisher.Publish(new BookingCreatedEvent
            {
                BookingId = booking.Id,
                RoomId = booking.RoomId,
                GuestName = booking.GuestName,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.TotalPrice
            });

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id },
                ToResponse(booking));
        }

        // ── Huỷ booking + publish event ─────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            if (booking.Status == BookingStatus.Cancelled)
                return BadRequest(new { error = "Booking đã bị huỷ rồi" });

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _publisher.Publish(new BookingCancelledEvent
            {
                BookingId = booking.Id,
                RoomId = booking.RoomId
            });

            return NoContent();
        }

        private static BookingResponse ToResponse(Booking b) => new(
            b.Id, b.RoomId, b.GuestName, b.GuestEmail,
            b.CheckIn, b.CheckOut, b.TotalPrice,
            b.NightCount, b.Status.ToString(), b.CreatedAt);
    }
}
