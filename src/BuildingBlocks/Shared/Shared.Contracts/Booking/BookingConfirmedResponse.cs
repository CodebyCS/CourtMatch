using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Booking
{
    // A Games.API chama a Booking.API antes de registar um jogo, para confirmar que a reserva existe e está confirmada.

    /// <summary>
    /// Represents a confirmed booking returned by the Booking API
    /// and used by the Game API to associate a game with a booking.
    /// </summary>
    public class BookingConfirmedResponse
    {
        /// <summary>Gets or sets the booking identifier.</summary>
        public Guid BookingId { get; set; }
        /// <summary>Gets or sets the identifier of the user who made the booking.</summary>
        public Guid UserId { get; set; }
        /// <summary>Gets or sets the identifier of the booked court.</summary>
        public Guid CourtId { get; set; }
        /// <summary>Gets or sets the booking date.</summary>
        public DateTime Date { get; set; }
        /// <summary>Gets or sets the booking status.</summary>
        public string Status { get; set; } = string.Empty;
    }
}
