using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Repositories
{
    public interface ICourtRepository
    {
        Task<IEnumerable<Court>> GetAllAsync(CancellationToken cancellationToken);
        Task<Court?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Court court, CancellationToken cancellationToken);
        Task UpdateAsync(Court court, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
