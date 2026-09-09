using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.DTOs;

namespace Booking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<Guid> CreateBookingAsync(CreateBookingDto createBookingDto, Guid hostPlayerId);
        Task<BookingDto> GetBookingByIdAsync(Guid bookingId, Guid userId);
        Task<BookingDto> UpdateBookingAsync(Guid bookingId, UpdateBookingDto updateBookingDto, Guid userId);
        Task<bool> DeleteBookingAsync(Guid bookingId, Guid userId);
        Task<BookingDto> AddEquipmentAsync(Guid bookingId, AddBookingEquipmentDto addEquipmentDto, Guid userId);
        Task<BookingDto> RemoveEquipmentAsync(Guid bookingId, Guid equipmentId, Guid userId);
        Task<BookingDto> ConfirmBookingAsync(Guid bookingId, Guid userId);
        Task<BookingDto> CancelBookingAsync(Guid bookingId, Guid userId);
        Task<IEnumerable<BookingDto>> GetBookingsByCourtAsync(Guid courtId);
        Task<IEnumerable<BookingDto>> GetMyBookingsAsync(Guid hostPlayerId);
        Task<IEnumerable<BookingDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}