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

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            try
            {
                var bookingId = await _bookingService.CreateBookingAsync(createBookingDto);
                return CreatedAtAction(nameof(CreateBooking), new { id = bookingId }, new { id = bookingId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
