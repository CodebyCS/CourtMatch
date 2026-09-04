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

        public async Task<IEnumerable<Court>> GetAllAsync()
        {
            return await _context.Courts.ToListAsync();
        }

        public async Task AddAsync(Court court)
        {
            await _context.Courts.AddAsync(court);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Court court)
        {
            _context.Courts.Update(court);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var court = await _context.Courts.FindAsync(id);
            if (court != null)
            {
                _context.Courts.Remove(court);
                await _context.SaveChangesAsync();
            }
        }
    }
}
