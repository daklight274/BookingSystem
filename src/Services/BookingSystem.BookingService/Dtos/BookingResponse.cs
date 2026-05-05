namespace BookingSystem.BookingService.Dtos
{
    public record BookingResponse(
        string Id,
        string RoomId,
        string GuestName,
        string GuestEmail,
        DateTime CheckIn,
        DateTime CheckOut,
        decimal TotalPrice,
        int NightCount,
        string Status,
        DateTime CreatedAt
    );
}
