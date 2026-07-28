using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.DTOs
{
    public class CourtDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsIndoor { get; set; }
        public decimal PricePerHour { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
