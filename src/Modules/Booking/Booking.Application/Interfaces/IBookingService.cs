using System;
using System.Threading.Tasks;
using Booking.Application.DTOs;

namespace Booking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<Guid> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingDto> GetBookingByIdAsync(Guid bookingId);
        Task<BookingDto> UpdateBookingAsync(Guid bookingId, UpdateBookingDto updateBookingDto);
        Task<bool> DeleteBookingAsync(Guid bookingId);
        Task<BookingDto> AddEquipmentAsync(Guid bookingId, AddBookingEquipmentDto addEquipmentDto);
        Task<BookingDto> RemoveEquipmentAsync(Guid bookingId, Guid equipmentId);
    }
}