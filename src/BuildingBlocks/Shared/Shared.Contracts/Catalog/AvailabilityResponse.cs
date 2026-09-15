using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Catalog
{
    // Contrato do CheckAvailability a Catalog.API devolve, a Booking.API consome.
    // Se um dos dois mudar sozinho, a integração parte — por isso vive aqui.
    /// <summary>
    /// Represents the result of a court availability check returned by the Catalog API
    /// and consumed by the Booking API.
    /// </summary>
    public class AvailabilityResponse
    {
        /// <summary>Gets or sets whether the court is available.</summary>
        public bool IsAvailable { get; set; }
        /// <summary>Gets or sets the identifier of the checked court.</summary>
        public Guid CourtId { get; set; }
        /// <summary>Gets or sets the court price per hour.</summary>
        public decimal PricePerHour { get; set; }
    }
}
