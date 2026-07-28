using Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Services
{
    public interface ICourtService
    {
        Task<IEnumerable<CourtDto>> GetAllCourtsAsync(CancellationToken cancellationToken);
        Task CreateCourtAsync(CourtDto courtDto, CancellationToken cancellationToken);
        Task UpdateCourtAsync(CourtDto courtDto, CancellationToken cancellationToken);
        Task DeleteCourtAsync(Guid id, CancellationToken cancellationToken);
    }
}
