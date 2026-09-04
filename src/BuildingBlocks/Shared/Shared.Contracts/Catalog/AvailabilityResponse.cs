using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Catalog
{
    // Contrato do CheckAvailability a Catalog.API devolve, a Booking.API consome.
    // Se um dos dois mudar sozinho, a integração parte — por isso vive aqui.
    public class AvailabilityResponse
    {
        public bool IsAvailable { get; set; }
        public Guid CourtId { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
