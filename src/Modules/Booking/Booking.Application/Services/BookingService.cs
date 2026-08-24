using System;
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

            return new BookingDto(
                booking.Id,
                booking.CourtId,
                booking.HostPlayerId,
                booking.StartTime,
                booking.EndTime,
                booking.CourtPrice,
                booking.TotalPrice,
                booking.Status.ToString()
            );


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

            return new BookingDto(
                booking.Id,
                booking.CourtId,
                booking.HostPlayerId,
                booking.StartTime,
                booking.EndTime,
                booking.CourtPrice,
                booking.TotalPrice,
                booking.Status.ToString()
            );
        }

        public async Task<bool>DeleteBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            _bookingRepository.Delete(booking);
            return true;
        }
    }
}