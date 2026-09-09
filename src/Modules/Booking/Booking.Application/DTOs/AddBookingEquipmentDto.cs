namespace Booking.Application.DTOs
{
    public record AddBookingEquipmentDto(
        int Quantity,
        Guid EquipmentId,
        decimal UnitPrice
    );
}
