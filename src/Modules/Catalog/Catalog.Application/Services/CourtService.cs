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
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository _courtRepository;

        public CourtService(ICourtRepository courtRepository)
        {
            _courtRepository = courtRepository;
        }

        public async Task<IEnumerable<CourtDto>> GetAllCourtsAsync(CancellationToken cancellationToken)
        {
            var courts = await _courtRepository.GetAllAsync();

            return courts.Select(c => new CourtDto
            {
                Id = c.Id,
                Name = c.Name,
                IsIndoor = c.IsIndoor,
                PricePerHour = c.PricePerHour,
                Status = c.Status
            }).ToList();
        }

        public async Task CreateCourtAsync(CourtDto courtDto, CancellationToken cancellationToken)
        {
            var court = new Court
            {
                Id = courtDto.Id == Guid.Empty ? Guid.NewGuid() : courtDto.Id,
                Name = courtDto.Name,
                IsIndoor = courtDto.IsIndoor,
                PricePerHour = courtDto.PricePerHour,
                Status = string.IsNullOrEmpty(courtDto.Status) ? "Available" : courtDto.Status
            };

            await _courtRepository.AddAsync(court);
        }

        public async Task UpdateCourtAsync(CourtDto courtDto, CancellationToken cancellationToken)
        {
            var court = new Court
            {
                Id = courtDto.Id,
                Name = courtDto.Name,
                IsIndoor = courtDto.IsIndoor,
                PricePerHour = courtDto.PricePerHour,
                Status = courtDto.Status
            };

            await _courtRepository.UpdateAsync(court);
        }

        public async Task DeleteCourtAsync(Guid id, CancellationToken cancellationToken)
        {
            await _courtRepository.DeleteAsync(id);
        }
    }
}
