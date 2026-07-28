using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _timeSlotRepository;

        public TimeSlotService(ITimeSlotRepository timeSlotRepository)
        {
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<IEnumerable<TimeSlotDto>> GetAllTimeSlotsAsync(CancellationToken cancellationToken)
        {
            var timeSlots = await _timeSlotRepository.GetAllAsync();

            return timeSlots.Select(t => new TimeSlotDto
            {
                Id = t.Id,
                Name = t.Name,
                StartTime = t.StartTime,
                EndTime = t.EndTime
            }).ToList();
        }

        public async Task CreateTimeSlotAsync(TimeSlotDto timeSlotDto, CancellationToken cancellationToken)
        {
            var timeSlot = new TimeSlot
            {
                Id = timeSlotDto.Id == Guid.Empty ? Guid.NewGuid() : timeSlotDto.Id,
                Name = timeSlotDto.Name,
                StartTime = timeSlotDto.StartTime,
                EndTime = timeSlotDto.EndTime
            };

            await _timeSlotRepository.AddAsync(timeSlot);
        }

        public async Task UpdateTimeSlotAsync(TimeSlotDto timeSlotDto, CancellationToken cancellationToken)
        {
            var timeSlot = new TimeSlot
            {
                Id = timeSlotDto.Id,
                Name = timeSlotDto.Name,
                StartTime = timeSlotDto.StartTime,
                EndTime = timeSlotDto.EndTime
            };

            await _timeSlotRepository.UpdateAsync(timeSlot);
        }

        public async Task DeleteTimeSlotAsync(Guid id, CancellationToken cancellationToken)
        {
            await _timeSlotRepository.DeleteAsync(id);
        }
    }
}
