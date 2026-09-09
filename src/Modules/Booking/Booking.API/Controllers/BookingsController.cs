using System;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shared.Contracts.Exceptions;

namespace Booking.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                 var userId = GetCurrentUserId();

            var bookingId = await _bookingService.CreateBookingAsync(
                dto,
                userId);

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = bookingId },
                new { id = bookingId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // LIST — por quadra (?courtId=) ou por intervalo (?startDate=&endDate=)
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetBookings(
            [FromQuery] Guid? courtId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            if (courtId.HasValue)
            {
                var byCourt = await _bookingService.GetBookingsByCourtAsync(courtId.Value);
                return Ok(byCourt);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                try
                {
                    var byRange = await _bookingService.GetBookingsByDateRangeAsync(startDate.Value, endDate.Value);
                    return Ok(byRange);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            return BadRequest(new { message = "Informe courtId, ou startDate e endDate." });
        }

        // GET /api/bookings/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();

            var bookings = await _bookingService.GetMyBookingsAsync(userId);

            return Ok(bookings);
        }

        // GET /api/Bookings/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _bookingService.GetBookingByIdAsync(
                id,
                userId);
            
            return Ok(booking);
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] UpdateBookingDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var updatedBooking = await _bookingService.UpdateBookingAsync(
                    id,
                    dto,
                    userId);

                return Ok(updatedBooking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var userId = GetCurrentUserId();

            var success = await _bookingService.DeleteBookingAsync(
                id,
                userId);
            if (!success) return NotFound();

            return NoContent();
        }

        // CONFIRM
        [HttpPatch("{id:guid}/confirm")]
        public async Task<IActionResult> ConfirmBooking(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _bookingService.ConfirmBookingAsync(
                id,
                userId);

            return Ok(booking);
        }

        // CANCEL
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _bookingService.CancelBookingAsync(
                id,
                userId);

            return Ok(booking);
        }

        // ADD EQUIPMENT
        [HttpPost("{id:guid}/equipments")]
        public async Task<IActionResult> AddEquipment(Guid id, [FromBody] AddBookingEquipmentDto dto)
        {
            var userId = GetCurrentUserId();

            var updatedBooking = await _bookingService.AddEquipmentAsync(
                id,
                dto,
                userId);

            return Ok(updatedBooking);
        }

        // REMOVE EQUIPMENT
        [HttpDelete("{id:guid}/equipments/{equipmentId:guid}")]
        public async Task<IActionResult> RemoveEquipment(Guid id, Guid equipmentId)
        {
            var userId = GetCurrentUserId();

            var updatedBooking = await _bookingService.RemoveEquipmentAsync(
                id,
                equipmentId,
                userId);

            return Ok(updatedBooking);
        }

        private Guid GetCurrentUserId()
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(subject, out var userId))
            {
                throw new UnauthorizedAppException(
                    "O token não contém um identificador de utilizador válido.");
            }

            return userId;
        }
    }
}