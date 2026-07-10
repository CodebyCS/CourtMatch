using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Booking
{
    internal class BookCourtRequest
    {
        public Guid CourtId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<Guid> ExtraEquipmentIds { get; set; } = new();
    }
}
