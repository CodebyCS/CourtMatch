using Catalog.Application.DTOs;
using Catalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _courtService.GetAllCourtsAsync(cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourtDto dto, CancellationToken cancellationToken)
        {
            await _courtService.CreateCourtAsync(dto, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CourtDto dto, CancellationToken cancellationToken)
        {
            try
            {
                await _courtService.UpdateCourtAsync(dto, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _courtService.DeleteCourtAsync(id, cancellationToken);
            return Ok();
        }
    }
}