using Booking.Domain.Repositories;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Booking> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.Equipments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.Bookings
                .Include(b => b.Equipments)
                .Where(b => b.CourtId == courtId)
                .ToListAsync();
        }

        public async Task AddAsync(Domain.Entities.Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public void Update(Domain.Entities.Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
        }

        public void Delete(Domain.Entities.Booking booking)
        {
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
        }

        public async Task<int> GetRentedEquipmentCountAsync(Guid equipmentId, DateTime startTime, DateTime endTime)
        {
            return await _context.Bookings
                .Where(b => b.StartTime < endTime &&
                            b.EndTime > startTime &&
                            b.Status != Domain.Enums.BookingStatus.Cancelled)
                .SelectMany(b => b.Equipments)
                .Where(e => e.EquipmentId == equipmentId)
                .SumAsync(e => e.Quantity);
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Include(b => b.Equipments)
                .Where(b => b.StartTime < endDate && b.EndTime > startDate)
                .ToListAsync();
        }
    }
}