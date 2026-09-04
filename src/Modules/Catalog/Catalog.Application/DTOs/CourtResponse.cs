using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Domain.Entities;

namespace Catalog.Application.DTOs
{
    public class CourtResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsIndoor { get; set; }

        public decimal PricePerHour { get; set; }
    }
}
