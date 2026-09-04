namespace Booking.Application.DTOs
{
    public record CreateBookingDto(
        Guid CourtId,
        Guid HostPlayerId,
        DateTime StartTime,
        DateTime EndTime,
        decimal CourtPrice,
        decimal TotalPrice
        );
}
