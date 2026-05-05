using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Shared.Contracts
{
    public record BookingCreatedEvent
    {
        public string BookingId { get; init; }
        public string RoomId { get; init; }
        public string GuestName { get; init; } = "";
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }
        public decimal TotalPrice { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
