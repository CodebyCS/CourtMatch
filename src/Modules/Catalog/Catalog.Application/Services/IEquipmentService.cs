using Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Services
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAllEquipmentsAsync(CancellationToken cancellationToken);
        Task CreateEquipmentAsync(EquipmentDto equipmentDto, CancellationToken cancellationToken);
        Task UpdateEquipmentAsync(EquipmentDto equipmentDto, CancellationToken cancellationToken);
        Task DeleteEquipmentAsync(Guid id, CancellationToken cancellationToken);
    }
}
