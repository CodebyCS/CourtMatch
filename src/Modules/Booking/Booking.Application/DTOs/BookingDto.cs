using System;

namespace Booking.Application.DTOs
{
    public record BookingDto(
        Guid Id,
        Guid CourtId,
        Guid HostPlayerId,
        DateTime StarTime,
        DateTime EndTime,
        decimal CourtPrice,
        decimal TotalPrice,
        string Status
    );
}