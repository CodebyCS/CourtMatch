using Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Services
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDto>> GetAllTimeSlotsAsync(CancellationToken cancellationToken);
        Task CreateTimeSlotAsync(TimeSlotDto timeSlotDto, CancellationToken cancellationToken);
        Task UpdateTimeSlotAsync(TimeSlotDto timeSlotDto, CancellationToken cancellationToken);
        Task DeleteTimeSlotAsync(Guid id, CancellationToken cancellationToken);
    }
}
