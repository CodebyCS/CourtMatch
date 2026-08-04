using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid CourtId { get; private set; }
        public Guid HostPlayerId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public decimal CourtPrice { get; private set; } 
        public decimal TotalPrice { get; private set; } 
        public BookingStatus Status { get; private set; }

 
        private readonly List<BookingEquipment> _equipments = new();


        public IReadOnlyCollection<BookingEquipment> Equipments => _equipments.AsReadOnly();

        public Booking(Guid courtId, Guid hostPlayerId, DateTime startTime, DateTime endTime, decimal courtPrice)
        {
            if (endTime <= startTime)
                throw new ArgumentException("O horário de fim tem de ser posterior ao horário de início.");

            Id = Guid.NewGuid();
            CourtId = courtId;
            HostPlayerId = hostPlayerId;
            StartTime = startTime;
            EndTime = endTime;
            CourtPrice = courtPrice;
            Status = BookingStatus.Pending;

            RecalculateTotalPrice();
        }



        public void AddEquipment(Guid equipmentId, int quantity, decimal unitPrice)
        {
            if (Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Não pode adicionar equipamentos a uma reserva cancelada.");

            var existingEquipment = _equipments.FirstOrDefault(e => e.EquipmentId == equipmentId);

            if (existingEquipment != null)
            {
                existingEquipment.UpdateQuantity(existingEquipment.Quantity + quantity);
            }
            else
            {
                var newEquipment = new BookingEquipment(this.Id, equipmentId, quantity, unitPrice);
                _equipments.Add(newEquipment);
            }

            RecalculateTotalPrice();
        }

        public void RemoveEquipment(Guid equipmentId)
        {
            var existingEquipment = _equipments.FirstOrDefault(e => e.EquipmentId == equipmentId);
            if (existingEquipment != null)
            {
                _equipments.Remove(existingEquipment);
                RecalculateTotalPrice();
            }
        }

        private void RecalculateTotalPrice()
        {
            
            TotalPrice = CourtPrice + _equipments.Sum(e => e.TotalPrice);
        }


        public void PaymentCompleted()
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidOperationException("Apenas reservas pendentes podem ser confirmadas.");

            Status = BookingStatus.Confirmed;
        }

        public void PaymentCancelled()
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidOperationException("Apenas reservas pendentes podem ser canceladas.");

            Status = BookingStatus.Cancelled;
        }

        public void UpdateSchedule(DateTime newStartTime, DateTime newEndTime)
        {
            if (newEndTime <= newStartTime)
                throw new ArgumentException("O horário de fim tem de ser posterior ao horário de início.");

            StartTime = newStartTime;
            EndTime = newEndTime;
        }
    }
}