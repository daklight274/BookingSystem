using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Shared.Contracts
{
    public record BookingCancelledEvent
    {
        public string BookingId { get; init; }
        public string RoomId { get; init; }
    }
}
