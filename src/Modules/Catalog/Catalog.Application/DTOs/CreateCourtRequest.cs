using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.DTOs
{
    public class CreateCourtRequest
    {
        public string Name { get; set; }

        public bool IsIndoor { get; set; }

        public decimal PricePerHour { get; set; }
    }
}
