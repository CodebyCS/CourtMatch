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

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="dto">Data required to create the booking (court, schedule, etc.).</param>
        /// <returns>Returns the route to the newly created booking.</returns>
        /// <response code="201">Booking created successfully.</response>
        /// <response code="400">Invalid data provided or schedule conflict.</response>
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

        /// <summary>
        /// Lists bookings. Allows filtering by a specific court or a date range. (Restricted to Managers).
        /// </summary>
        /// <param name="courtId">Unique identifier of the court (optional).</param>
        /// <param name="startDate">Start date of the range (optional).</param>
        /// <param name="endDate">End date of the range (optional).</param>
        /// <returns>Returns a list of bookings matching the search criteria.</returns>
        /// <response code="200">List of bookings returned successfully.</response>
        /// <response code="400">No valid search criteria provided or invalid dates.</response>
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

            return BadRequest(new { message = "Provide either courtId, or both startDate and endDate." });
        }

        /// <summary>
        /// Retrieves all bookings associated with the currently authenticated user.
        /// </summary>
        /// <returns>Returns the list of user bookings.</returns>
        /// <response code="200">List of bookings returned successfully.</response>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();
            var bookings = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Retrieves the details of a specific booking by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the booking.</param>
        /// <returns>Returns the detailed data of the specified booking.</returns>
        /// <response code="200">Booking found and returned successfully.</response>
        /// <response code="404">Booking not found or does not belong to the user.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _bookingService.GetBookingByIdAsync(
                id,
                userId);

            return Ok(booking);
        }

        /// <summary>
        /// Updates an existing booking's data.
        /// </summary>
        /// <param name="id">Unique identifier of the booking to be updated.</param>
        /// <param name="dto">New data for updating the booking.</param>
        /// <returns>Returns the updated booking.</returns>
        /// <response code="200">Booking updated successfully.</response>
        /// <response code="400">Invalid data provided for the update.</response>
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

        /// <summary>
        /// Permanently deletes a specific booking.
        /// </summary>
        /// <param name="id">Unique identifier of the booking to be deleted.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Booking deleted successfully.</response>
        /// <response code="404">Booking not found.</response>
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

        /// <summary>
        /// Changes the status of a booking to confirmed.
        /// </summary>
        /// <param name="id">Unique identifier of the booking to be confirmed.</param>
        /// <returns>Returns the booking with the updated status.</returns>
        /// <response code="200">Booking confirmed successfully.</response>
        [HttpPatch("{id:guid}/confirm")]
        public async Task<IActionResult> ConfirmBooking(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _bookingService.ConfirmBookingAsync(
                id,
                userId);

            return Ok(booking);
        }

        /// <summary>
        /// Changes the status of a booking to canceled.
        /// </summary>
        /// <param name="id">Unique identifier of the booking to be canceled.</param>
        /// <returns>Returns the booking with the status updated to canceled.</returns>
        /// <response code="200">Booking canceled successfully.</response>
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _bookingService.CancelBookingAsync(
                id,
                userId);

            return Ok(booking);
        }

        /// <summary>
        /// Adds equipment (e.g., rackets, balls) to an existing booking.
        /// </summary>
        /// <param name="id">Unique identifier of the booking.</param>
        /// <param name="dto">Details of the equipment to be added and its quantity.</param>
        /// <returns>Returns the updated booking containing the new equipment.</returns>
        /// <response code="200">Equipment added successfully.</response>
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

        /// <summary>
        /// Removes equipment previously associated with a booking.
        /// </summary>
        /// <param name="id">Unique identifier of the booking.</param>
        /// <param name="equipmentId">Unique identifier of the equipment to be removed.</param>
        /// <returns>Returns the updated booking without the removed equipment.</returns>
        /// <response code="200">Equipment removed successfully.</response>
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

        /// <summary>
        /// Extracts and validates the user ID from the current JWT token claims.
        /// </summary>
        /// <returns>Unique identifier (Guid) of the authenticated user.</returns>
        /// <exception cref="UnauthorizedAppException">Thrown if the token does not contain a valid user identifier.</exception>
        private Guid GetCurrentUserId()
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(subject, out var userId))
            {
                throw new UnauthorizedAppException(
                    "The token does not contain a valid user identifier.");
            }

            return userId;
        }
    }
}