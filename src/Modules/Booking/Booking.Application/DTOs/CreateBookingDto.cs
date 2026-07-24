namespace Booking.Application.DTOs
{
    public record CreateBookingDto(
        Guid CourtId,
        Guid HostPlayerId,
        DateTime Schedule,
        decimal TotalPrice
        );
}
