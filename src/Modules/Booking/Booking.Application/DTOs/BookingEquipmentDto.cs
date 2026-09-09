using System;

namespace Booking.Application.DTOs
{
    public record BookingEquipmentDto(
        Guid Id,
        Guid EquipmentId,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
}
