namespace BookingSystem.BookingService.Dtos
{
    public record CreateBookingRequest(
        string RoomId,
        string GuestName,
        string GuestEmail,
        string GuestPhone,
        DateTime CheckIn,
        DateTime CheckOut,
        decimal PricePerNight,
        string? Notes
    );
}
