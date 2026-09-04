using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;

namespace Booking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Guid> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            //validate date
            if (createBookingDto.StartTime < DateTime.UtcNow)
            {
                throw new ArgumentException("Start time cannot be in the past.");
            }
            var booking = new Domain.Entities.Booking(
                createBookingDto.CourtId,
                createBookingDto.HostPlayerId,
                createBookingDto.StartTime,
                createBookingDto.EndTime,
                createBookingDto.CourtPrice
            );

            await _bookingRepository.AddAsync(booking);

            return booking.Id;
        }

        public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

            return MapToDto(booking);
        }

        public async Task<BookingDto> UpdateBookingAsync(Guid bookingId, UpdateBookingDto updateBookingDto)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

            if (updateBookingDto.StartTime < DateTime.UtcNow)
            {
                throw new ArgumentException("Start time cannot be in the past.");
            }

            // Atualiza os dados da Entidade
            booking.UpdateSchedule(updateBookingDto.StartTime, updateBookingDto.EndTime);

            _bookingRepository.Update(booking);

            return MapToDto(booking);
        }

        public async Task<bool>DeleteBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            _bookingRepository.Delete(booking);
            return true;
        }

        public async Task<BookingDto> AddEquipmentAsync(Guid bookingId, AddBookingEquipmentDto addEquipmentDto)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

            booking.AddEquipment(addEquipmentDto.EquipmentId, addEquipmentDto.Quantity, addEquipmentDto.UnitPrice);

            _bookingRepository.Update(booking);

            return MapToDto(booking);
        }

        public async Task<BookingDto> RemoveEquipmentAsync(Guid bookingId, Guid equipmentId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

            booking.RemoveEquipment(equipmentId);

            _bookingRepository.Update(booking);

            return MapToDto(booking);
        }

        public async Task<BookingDto> ConfirmBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

            booking.PaymentCompleted();

            _bookingRepository.Update(booking);

            return MapToDto(booking);
        }

        public async Task<BookingDto> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

            booking.PaymentCancelled();

            _bookingRepository.Update(booking);

            return MapToDto(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByCourtAsync(Guid courtId)
        {
            var bookings = await _bookingRepository.GetByCourtIdAsync(courtId);

            return bookings.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
            {
                throw new ArgumentException("End date must be after start date.");
            }

            var bookings = await _bookingRepository.GetBookingsByDateRangeAsync(startDate, endDate);

            return bookings.Select(MapToDto).ToList();
        }

        private static BookingDto MapToDto(Domain.Entities.Booking booking)
        {
            var equipments = booking.Equipments
                .Select(e => new BookingEquipmentDto(e.Id, e.EquipmentId, e.Quantity, e.UnitPrice, e.TotalPrice))
                .ToList();

            return new BookingDto(
                booking.Id,
                booking.CourtId,
                booking.HostPlayerId,
                booking.StartTime,
                booking.EndTime,
                booking.CourtPrice,
                booking.TotalPrice,
                booking.Status.ToString(),
                equipments
            );
        }
    }
}