using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Entities.Booking> GetByIdAsync(Guid id);
        Task<IEnumerable<Entities.Booking>> GetByCourtIdAsync(Guid courtId);
        Task AddAsync(Entities.Booking booking);
        void Update(Entities.Booking booking);
        void Delete(Entities.Booking booking);
        Task<int> GetRentedEquipmentCountAsync(Guid equipmentId, DateTime startTime, DateTime endTime);
    }
}