using BookingSystem.RoomService.Data;
using BookingSystem.RoomService.Dtos;
using BookingSystem.RoomService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.RoomService.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomDbContext _db;
        public RoomController(RoomDbContext db) => _db = db;

        // GET /api/rooms?available=true&type=Double
        [HttpGet]
        public async Task<IActionResult>GetAll(
            [FromQuery] bool? available,
            [FromQuery] RoomType? type)
        {
            var q = _db.Rooms.AsQueryable();
            if (available.HasValue) q = q.Where(r => r.IsAvailable == available);
            if (type.HasValue) q = q.Where(r => r.Type == type);
            return Ok(await q.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetById(string id)
        {
            var room = await _db.Rooms.FindAsync(id);
            return room is null ? NotFound() : Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomRequest req)
        {
            var room = new Room
            {
                RoomNumber = req.RoomNumber,
                Type = req.Type,
                PricePerNight = req.PricePerNight,
                Description = req.Description,
                MaxGuests = req.MaxGuests
            };
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        // Đổi trạng thái available — gọi nội bộ từ Hangfire/event
        [HttpPut("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(
            string id, [FromBody] bool isAvailable)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room is null) return NotFound();
            room.IsAvailable = isAvailable;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
