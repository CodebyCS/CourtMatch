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
        Task<IEnumerable<Court>> GetAllAsync();
        Task AddAsync(Court court);
        Task UpdateAsync(Court court);
        Task DeleteAsync(Guid id);
    }
}
