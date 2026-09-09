using Catalog.Application.DTOs;
using Catalog.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FacilitiesCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourtsController : ControllerBase
    {
        private readonly ICourtService _courtService;

        public CourtsController(ICourtService courtService)
        {
            _courtService = courtService;
        }

        // GET: /api/courts
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _courtService.GetAllCourtsAsync(cancellationToken);
            return Ok(result);
        }

        // GET: /api/courts/{id}
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var court = await _courtService.GetCourtByIdAsync(id, cancellationToken);

            return Ok(court);
        }

        // POST: /api/courts
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourtRequest request, CancellationToken cancellationToken)
        {
            var court = await _courtService.CreateCourtAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = court.Id }, court);
        }

        // PUT: /api/courts/{id}
        [Authorize(Roles = "Manager")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourtRequest request, CancellationToken cancellationToken)
        {
            await _courtService.UpdateCourtAsync(id, request, cancellationToken);
            return NoContent();
        }

        // DELETE: /api/courts/{id}
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _courtService.DeleteCourtAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: /api/courts/{id}/block
        [Authorize(Roles = "Manager")]
        [HttpPatch("{id:guid}/block")]
        public async Task<IActionResult> Block(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _courtService.BlockCourtAsync(id, cancellationToken);

            return NoContent();
        }

        // GET: /api/courts/{id}/availability?date=2026-09-10&startTime=18:00:00
        [AllowAnonymous]
        [HttpGet("{id:guid}/availability")]
        public async Task<IActionResult> CheckAvailability(
            Guid id,
            [FromQuery] DateTime date,
            [FromQuery] TimeSpan startTime,
            CancellationToken cancellationToken)
        {
            var availability = await _courtService.CheckAvailabilityAsync(
                id,
                date,
                startTime,
                cancellationToken);

            return Ok(availability);
        }
    }
}