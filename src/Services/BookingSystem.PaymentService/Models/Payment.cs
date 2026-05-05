namespace BookingSystem.PaymentService.Models
{
    public class Payment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BookingId { get; set; }
        public string RoomId { get; set; }   // cần để publish PaymentFailedEvent
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public enum PaymentStatus
    {
        Pending = 0,  // chờ thanh toán
        Completed = 1,  // thanh toán thành công
        Failed = 2,  // thanh toán thất bại
        Refunded = 3   // đã hoàn tiền
    }

    public enum PaymentMethod
    {
        Cash = 1,
        CreditCard = 2,
        BankTransfer = 3,
        EWallet = 4
    }
}
