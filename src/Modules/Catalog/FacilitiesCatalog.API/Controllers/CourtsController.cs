using Catalog.Application.DTOs;
using Catalog.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _courtService.GetAllCourtsAsync(cancellationToken);
            return Ok(result);
        }

        // GET: /api/courts/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById (Guid id, CancellationToken cancellationToken)
        {
            var court = await _courtService.GetCourtByIdAsync(id, cancellationToken);

            return Ok(court);
        }

        // POST: /api/courts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourtRequest request, CancellationToken cancellationToken)
        {
            var court = await _courtService.CreateCourtAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = court.Id }, court);
        }

        // PUT: /api/courts/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourtRequest request, CancellationToken cancellationToken)
        {
            await _courtService.UpdateCourtAsync(id, request, cancellationToken);
            return NoContent();
        }

        // DELETE: /api/courts/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _courtService.DeleteCourtAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: /api/courts/{id}/block
        [HttpPatch("{id:guid}/block")]
        public async Task<IActionResult> Block(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _courtService.BlockCourtAsync(id, cancellationToken);

            return NoContent();
        }
    }
}