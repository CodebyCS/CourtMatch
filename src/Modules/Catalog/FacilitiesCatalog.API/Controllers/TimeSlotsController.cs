using Catalog.Application.DTOs;
using Catalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacilitiesCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotsController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _timeSlotService.GetAllTimeSlotsAsync(cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimeSlotDto dto, CancellationToken cancellationToken)
        {
            await _timeSlotService.CreateTimeSlotAsync(dto, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TimeSlotDto dto, CancellationToken cancellationToken)
        {
            await _timeSlotService.UpdateTimeSlotAsync(dto, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _timeSlotService.DeleteTimeSlotAsync(id, cancellationToken);
            return Ok();
        }
    }
}
