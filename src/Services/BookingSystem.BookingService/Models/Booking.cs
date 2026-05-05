namespace BookingSystem.BookingService.Models
{
    public class Booking
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoomId { get; set; }
        public string GuestName { get; set; } = "";
        public string GuestEmail { get; set; } = "";
        public string GuestPhone { get; set; } = "";
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Computed — không lưu DB
        public int NightCount =>
            (int)(CheckOut - CheckIn).TotalDays;
    }

    public enum BookingStatus
    {
        Pending = 0,   // vừa tạo, chờ thanh toán
        Confirmed = 1,   // thanh toán thành công
        Cancelled = 2    // huỷ hoặc thanh toán thất bại
    }
}
