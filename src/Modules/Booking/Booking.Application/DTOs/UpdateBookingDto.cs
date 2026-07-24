using System;

namespace Booking.Application.DTOs
{
    public record UpdateBookingDto(
        DateTime Schedule,
        decimal TotalPrice
    );
}
