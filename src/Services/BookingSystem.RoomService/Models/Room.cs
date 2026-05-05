namespace BookingSystem.RoomService.Models
{
    public class Room
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoomNumber { get; set; } = "";
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Description { get; set; } = "";
        public int MaxGuests { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum RoomType
    {
        Single = 1,
        Double = 2,
        Suite = 3
    }
}
