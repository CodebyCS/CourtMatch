namespace Booking.Application.DTOs
{
    public record CreateBookingDto(
        Guid CourtId,
        //Guid HostPlayerId, // ----> O user autenticado receber o HostPlayerId atraves do JWT, nao pelo body.
        DateTime StartTime,
        DateTime EndTime
        // decimal CourtPrice, ----> o user poderia enviar qualquer preço, incluindo 0
        // decimal TotalPrice  ----> o user poderia enviar qualquer preço, incluindo 0
        );
}
