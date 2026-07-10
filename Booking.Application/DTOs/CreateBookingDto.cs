using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs
{
    public record CreateBookingDto(
        Guid CourtId,
        Guid HostPlayerId,
        DateTime Schedule,
        decimal TotalPrice
        );
}
