using System;
using System.Collections.Generic;

namespace Booking.Application.DTOs
{
    public record BookingDto(
        Guid Id,
        Guid CourtId,
        Guid HostPlayerId,
        DateTime StartTime,
        DateTime EndTime,
        decimal CourtPrice,
        decimal TotalPrice,
        string Status,
        List<BookingEquipmentDto> BookingEquipments
    );
}   