using System;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
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
                var bookingId = await _bookingService.CreateBookingAsync(dto);
                return CreatedAtAction(nameof(GetBookingById), new { id = bookingId }, new { id = bookingId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // LIST — por quadra (?courtId=) ou por intervalo (?startDate=&endDate=)
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

        // READ
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            return Ok(booking);
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] UpdateBookingDto dto)
        {
            try
            {
                var updatedBooking = await _bookingService.UpdateBookingAsync(id, dto);
                if (updatedBooking == null) return NotFound();

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
            var success = await _bookingService.DeleteBookingAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        // CONFIRM
        [HttpPatch("{id:guid}/confirm")]
        public async Task<IActionResult> ConfirmBooking(Guid id)
        {
            try
            {
                var booking = await _bookingService.ConfirmBookingAsync(id);
                if (booking == null) return NotFound();

                return Ok(booking);
            }
            catch (InvalidOperationException ex)
            {
                // A reserva não está Pending — transição de estado inválida.
                return Conflict(new { message = ex.Message });
            }
        }

        // CANCEL
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            try
            {
                var booking = await _bookingService.CancelBookingAsync(id);
                if (booking == null) return NotFound();

                return Ok(booking);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // ADD EQUIPMENT
        [HttpPost("{id:guid}/equipments")]
        public async Task<IActionResult> AddEquipment(Guid id, [FromBody] AddBookingEquipmentDto dto)
        {
            try
            {
                var updatedBooking = await _bookingService.AddEquipmentAsync(id, dto);
                if (updatedBooking == null) return NotFound();

                return Ok(updatedBooking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // REMOVE EQUIPMENT
        [HttpDelete("{id:guid}/equipments/{equipmentId:guid}")]
        public async Task<IActionResult> RemoveEquipment(Guid id, Guid equipmentId)
        {
            var updatedBooking = await _bookingService.RemoveEquipmentAsync(id, equipmentId);
            if (updatedBooking == null) return NotFound();

            return Ok(updatedBooking);
        }
    }
}