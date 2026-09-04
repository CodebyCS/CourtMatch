using Catalog.Application.DTOs;
using Catalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacilitiesCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentsController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentsController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _equipmentService.GetAllEquipmentsAsync(cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EquipmentDto dto, CancellationToken cancellationToken)
        {
            await _equipmentService.CreateEquipmentAsync(dto, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EquipmentDto dto, CancellationToken cancellationToken)
        {
            await _equipmentService.UpdateEquipmentAsync(dto, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _equipmentService.DeleteEquipmentAsync(id, cancellationToken);
            return Ok();
        }
    }

}
