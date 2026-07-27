using Catalog.Domain.Repositories;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly CatalogDbContext _context;

        // Injetamos o DbContext no construtor
        public TimeSlotRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimeSlot>> GetAllAsync()
        {
            return await _context.TimeSlots.ToListAsync();
        }

        public async Task AddAsync(TimeSlot timeSlot)
        {
            await _context.TimeSlots.AddAsync(timeSlot);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null) return;

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
        }
    }
}
