using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Shared.Contracts
{
    public record PaymentCompletedEvent
    {
        public string PaymentId { get; init; }
        public string BookingId { get; init; }
        public decimal Amount { get; init; }
        public string Method { get; init; } = "";
        public DateTime PaidAt { get; init; } = DateTime.UtcNow;
    }
}
