using System;
using System.Collections.Generic;
using Catalog.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Domain.Repositories;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly CatalogDbContext _context;

        public EquipmentRepository(CatalogDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Equipment equipment)
        {
            await _context.Set<Equipment>().AddAsync(equipment);
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            return await _context.Set<Equipment>().ToListAsync();
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            _context.Set<Equipment>().Update(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var equipment = await _context.Set<Equipment>().FindAsync(id);
            if (equipment != null)
            {
                _context.Set<Equipment>().Remove(equipment);
                await _context.SaveChangesAsync();
            }
        }

    }
}
