using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Shared.Contracts.Exceptions;

namespace Booking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        private readonly ICatalogAvailabilityClient _catalogAvailabilityClient;

        public BookingService(IBookingRepository bookingRepository, ICatalogAvailabilityClient catalogAvailabilityClient)
        {
            _bookingRepository = bookingRepository;
            _catalogAvailabilityClient = catalogAvailabilityClient;
        }

        public async Task<Guid> CreateBookingAsync(CreateBookingDto createBookingDto, Guid hostPlayerId)
        {
            //validate date
            if (createBookingDto.StartTime < DateTime.UtcNow)
            {
                throw new ArgumentException("Start time cannot be in the past.");
            }

            var availability = await _catalogAvailabilityClient.GetAvailabilityAsync(
                createBookingDto.CourtId,
                createBookingDto.StartTime);

            if (availability is null)
            {
                throw new ArgumentException("O campo indicado não existe.");
            }

            if (!availability.IsAvailable)
            {
                throw new ArgumentException(
                    "O campo não está disponível para a data e hora selecionadas.");
            }

            if (createBookingDto.EndTime <= createBookingDto.StartTime)
            {
                throw new ArgumentException(
                    "O horário de fim tem de ser posterior ao horário de início.");
            }

            var hasOverlap = await _bookingRepository.HasOverlapAsync(
                createBookingDto.CourtId,
                createBookingDto.StartTime,
                createBookingDto.EndTime);

            if (hasOverlap)
            {
                throw new ArgumentException(
                    "Já existe uma reserva ativa que se sobrepõe a esse horário.");
            }

            var duration = createBookingDto.EndTime - createBookingDto.StartTime;

            var courtPrice = decimal.Round(
                availability.PricePerHour * (decimal)duration.TotalHours,
                2,
                MidpointRounding.AwayFromZero);

            var booking = new Domain.Entities.Booking(
                createBookingDto.CourtId,
                hostPlayerId,
                createBookingDto.StartTime,
                createBookingDto.EndTime,
                courtPrice);

            await _bookingRepository.AddAsync(booking);

            return booking.Id;
        }

        public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking is null)
            {
                throw new NotFoundException("Reserva não encontrada.");
            }

            return MapToDto(booking);
        }

        public async Task<BookingDto> UpdateBookingAsync(
            Guid bookingId,
            UpdateBookingDto updateBookingDto)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking is null)
            {
                throw new ArgumentException("A reserva não existe.");
            }

            if (updateBookingDto.StartTime < DateTime.UtcNow)
            {
                throw new ArgumentException(
                    "A data de início não pode estar no passado.");
            }

            if (updateBookingDto.EndTime <= updateBookingDto.StartTime)
            {
                throw new ArgumentException(
                    "O horário de fim tem de ser posterior ao horário de início.");
            }

            var hasOverlap = await _bookingRepository.HasOverlapAsync(
                booking.CourtId,
                updateBookingDto.StartTime,
                updateBookingDto.EndTime,
                booking.Id);

            if (hasOverlap)
            {
                throw new ArgumentException(
                    "Já existe uma reserva ativa que se sobrepõe a esse horário.");
            }

            var availability = await _catalogAvailabilityClient.GetAvailabilityAsync(
                booking.CourtId,
                updateBookingDto.StartTime);

            if (availability is null)
            {
                throw new ArgumentException("O campo indicado não existe.");
            }

            if (!availability.IsAvailable)
            {
                throw new ArgumentException(
                    "O campo não está disponível para a data e hora selecionadas.");
            }

            var duration = updateBookingDto.EndTime - updateBookingDto.StartTime;

            var courtPrice = decimal.Round(
                availability.PricePerHour * (decimal)duration.TotalHours,
                2,
                MidpointRounding.AwayFromZero);

            booking.UpdateSchedule(
                updateBookingDto.StartTime,
                updateBookingDto.EndTime,
                courtPrice);

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

        public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync(
            Guid hostPlayerId)
        {
            var bookings = await _bookingRepository
                .GetByHostPlayerIdAsync(hostPlayerId);

            return bookings.Select(MapToDto).ToList();
        }
    }
}