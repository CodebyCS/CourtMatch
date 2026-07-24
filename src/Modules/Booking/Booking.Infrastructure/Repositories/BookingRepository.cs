using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
       private BookingDbContext _context;
        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }
        
        public async Task<Booking.Domain.Entities.Booking> GetByIdAsync(Guid id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<IEnumerable<Booking.Domain.Entities.Booking>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.Bookings.Where(b => b.CourtId == courtId).ToListAsync();
        }

        public async Task AddAsync(Booking.Domain.Entities.Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public void Update(Booking.Domain.Entities.Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
        }
    }
}
