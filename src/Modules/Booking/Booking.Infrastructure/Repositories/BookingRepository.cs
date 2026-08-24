using System.Linq;
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
            // ChangeTracker.Entries() triggers DetectChanges(), which would auto-attach
            // new BookingEquipment items as Modified (their Id is already a real Guid,
            // not the CLR default, so EF assumes they pre-exist). Disabling auto-detect
            // here lets us see only the entities that are genuinely already tracked
            // (loaded from the DB), so we can Add() the new ones explicitly as Added.
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                var trackedEquipmentIds = _context.ChangeTracker.Entries<BookingEquipment>()
                    .Select(e => e.Entity.Id)
                    .ToHashSet();

                foreach (var equipment in booking.Equipments)
                {
                    if (!trackedEquipmentIds.Contains(equipment.Id))
                    {
                        _context.BookingEquipments.Add(equipment);
                    }
                }
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }

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