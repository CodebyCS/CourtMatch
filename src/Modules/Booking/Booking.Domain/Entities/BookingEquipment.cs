using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Repositories;

namespace Booking.Domain.Entities
{
    public class BookingEquipment
    {
        public Guid Id { get; private set; }
        public Guid BookingId { get; private set; }
        public Guid EquipmentId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }

        public BookingEquipment(Guid bookingId, Guid equipmentId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("O preço não pode ser negativo.", nameof(unitPrice));

            Id = Guid.NewGuid();
            BookingId = bookingId;
            EquipmentId = equipmentId;
            Quantity = quantity;
            UnitPrice = unitPrice;

            CalculateTotalPrice();
        }


        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            Quantity = newQuantity;
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Quantity * UnitPrice;
        }
    }
}
