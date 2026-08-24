using System;

namespace Booking.Application.DTOs
{
    public record UpdateBookingDto(
        DateTime StartTime,
        DateTime EndTime,
        decimal CourtPrice
    );
}
