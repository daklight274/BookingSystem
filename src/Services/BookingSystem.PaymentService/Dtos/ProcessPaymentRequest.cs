using BookingSystem.PaymentService.Models;

namespace BookingSystem.PaymentService.Dtos
{
    public record ProcessPaymentRequest(
        string BookingId,
        string RoomId,
        decimal Amount,
        PaymentMethod Method,
        string? Notes
    );
}
