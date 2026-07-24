using System;

namespace Booking.Application.DTOs
{
    public record BookingDto(
        Guid Id,
        Guid CourtId,
        Guid HostPlayerId,
        DateTime Schedule,
        decimal TotalPrice,
        string Status
    );
}