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

        // GetAll
        /// <summary>Lists all padel courts.</summary>
        // GET: /api/courts
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _courtService.GetAllCourtsAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>Gets a court by its identifier.</summary>
        // GET: /api/courts/{id}
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var court = await _courtService.GetCourtByIdAsync(id, cancellationToken);

            return Ok(court);
        }
        
        /// <summary>Creates a new padel court. Requires the Manager role.</summary>
        // POST: /api/courts
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourtRequest request, CancellationToken cancellationToken)
        {
            var court = await _courtService.CreateCourtAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = court.Id }, court);
        }

        /// <summary>Updates a court's details and status. Requires the Manager role.</summary>
        // PUT: /api/courts/{id}
        [Authorize(Roles = "Manager")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourtRequest request, CancellationToken cancellationToken)
        {
            await _courtService.UpdateCourtAsync(id, request, cancellationToken);
            return NoContent();
        }

        /// <summary>Deletes a court by its identifier. Requires the Manager role.</summary>
        // DELETE: /api/courts/{id}
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _courtService.DeleteCourtAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>Marks a court as under maintenance. Requires the Manager role.</summary>
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

        /// <summary>Checks whether a court is available on the specified date and at the specified start time.</summary>
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