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
            if (dto.Schedule < DateTime.UtcNow)
            {
                throw new ArgumentException("Schedule date cannot be in the past.");
            }

            var booking = new Domain.Entities.Booking
            {
                dto.CourtId,
                dto.HostPlayerId,
                dto.Schedule,
                dto.TotalPrice
            };

            await _bookingRepository.AddAsysnc(booking);

            return booking.Id;
        }
        //Task<BookingDto> GetBookingByIdAsync(Guid bookingId);
        //Task<BookingDto> UpdateBookingAsync(Guid bookingId, UpdateBookingDto updateBookingDto);
        //Task<bool> DeleteBookingAsync(Guid bookingId);
    }
}