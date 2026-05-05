using BookingSystem.RoomService.Models;

namespace BookingSystem.RoomService.Dtos
{
    public class CreateRoomRequest
    {
        public string RoomNumber { get; set; }
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description { get; set; }
        public int MaxGuests { get; set; }
    }
}
