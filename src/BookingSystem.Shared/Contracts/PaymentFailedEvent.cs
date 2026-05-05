using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Shared.Contracts
{
    public record PaymentFailedEvent
    {
        public string BookingId { get; init; }
        public string RoomId { get; init; }
        public string Reason { get; init; } = "";
        public DateTime FailedAt { get; init; } = DateTime.UtcNow;
    }
}
