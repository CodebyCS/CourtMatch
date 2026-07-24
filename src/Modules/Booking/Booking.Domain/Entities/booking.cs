using System;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{

    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid CourtId { get; private set; }
        public Guid HostPlayerId { get; private set; }
        public DateTime Schedule { get; private set; }
        public decimal TotalPrice { get; private set; }
        public BookingStatus Status { get; private set; }


        public Booking(Guid courtId, Guid hostPlayerId, DateTime schedule, decimal totalPrice)
        {
            Id = Guid.NewGuid();
            CourtId = courtId;
            HostPlayerId = hostPlayerId;
            Schedule = schedule;
            TotalPrice = totalPrice;
            Status = BookingStatus.Pending;
        }

        public void PaymentCompleted()
        {
            if (Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Apenas reservas pendentes podem ser confirmadas.");
            }

            Status = BookingStatus.Confirmed;
        }


        public void PaymentCancelled()
        {
            if (Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Apenas reservas pendentes podem ser canceladas.");
            }

            Status = BookingStatus.Cancelled;
        }
    }
}