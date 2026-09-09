using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentService(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllEquipmentsAsync(CancellationToken cancellationToken)
        {
            var equipments = await _equipmentRepository.GetAllAsync();

            return equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Category = e.Category,
                Stock = e.Stock,
                RentalPrice = e.RentalPrice
            }).ToList();
        }

        public async Task CreateEquipmentAsync(EquipmentDto equipmentDto, CancellationToken cancellationToken)
        {
            var equipment = new Equipment
            {
                Id = equipmentDto.Id == Guid.Empty ? Guid.NewGuid() : equipmentDto.Id,
                Name = equipmentDto.Name,
                Category = equipmentDto.Category,
                Stock = equipmentDto.Stock,
                RentalPrice = equipmentDto.RentalPrice
            };

            await _equipmentRepository.AddAsync(equipment);
        }

        public async Task UpdateEquipmentAsync(EquipmentDto equipmentDto, CancellationToken cancellationToken)
        {
            var equipment = new Equipment
            {
                Id = equipmentDto.Id,
                Name = equipmentDto.Name,
                Category = equipmentDto.Category,
                Stock = equipmentDto.Stock,
                RentalPrice = equipmentDto.RentalPrice
            };

            await _equipmentRepository.UpdateAsync(equipment);
        }

        public async Task DeleteEquipmentAsync(Guid id, CancellationToken cancellationToken)
        {
            await _equipmentRepository.DeleteAsync(id);
        }
    }
}
