namespace BookingSystem.PaymentService.Dtos
{
    public record PaymentResponse(
        string Id,
        string BookingId,
        decimal Amount,
        string Status,
        string Method,
        DateTime CreatedAt,
        DateTime? PaidAt
    );
}
