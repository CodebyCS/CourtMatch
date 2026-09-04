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
        Task<IEnumerable<CourtResponse>> GetAllCourtsAsync(CancellationToken cancellationToken);
        Task<CourtResponse> GetCourtByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<CourtResponse> CreateCourtAsync(CreateCourtRequest request, CancellationToken cancellationToken);
        Task UpdateCourtAsync(Guid id, UpdateCourtRequest request, CancellationToken cancellationToken);
        Task DeleteCourtAsync(Guid id, CancellationToken cancellationToken);
        Task BlockCourtAsync(Guid id, CancellationToken cancellationToken);
    }
}
