using Catalog.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.DTOs
{
    public class UpdateCourtRequest
    {
        public string Name { get; set; } = string.Empty;

        public bool IsIndoor { get; set; }

        public decimal PricePerHour { get; set; }

        public CourtStatus Status { get; set; }
    }
}
