using System;
using System.Collections.Generic;
using Catalog.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Domain.Repository;
using Catalog.Domain.Entities;
using FacilitiesCatalog.API;

namespace Catalog.Infrastructure.Repositories
{
    public interface EquipmentRepository : IEquipmentRepository
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

     
    }
}
