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
    }
}