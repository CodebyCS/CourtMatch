using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Repositories
{
    public class CourtRepository : ICourtRepository
    {
        private readonly CatalogDbContext _context;
        public CourtRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Court>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Courts.AsNoTracking().ToListAsync(cancellationToken);
        }

        // Devolve a Entidade rastreada pelo EF Core
        public async Task<Court?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Courts.FirstOrDefaultAsync(C => C.Id == id, cancellationToken);
        }

        public async Task AddAsync(Court court, CancellationToken cancellationToken)
        {
            await _context.Courts.AddAsync(court, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Court court, CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var court = await _context.Courts.FirstOrDefaultAsync(C => C.Id == id, cancellationToken);
            if (court is null) return false;

            _context.Courts.Remove(court);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
