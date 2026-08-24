using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Booking
{
    // A Games.API chama a Booking.API antes de registar um jogo, para confirmar que a reserva existe e está confirmada.
    public class BookingConfirmedResponse
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourtId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
