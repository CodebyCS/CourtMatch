using Games.Domain.Entities.Enums;

namespace Game.Application.Entities
{
    public class Game
    {
        public Guid Id { get; private set; }
        public Guid BookingId { get; private set; }   // referência à reserva no Ordering.API
        public Guid FacilityId { get; private set; }  // referência ao campo no Catalog.API
        public DateTime ScheduledAt { get; private set; }
        public GameStatus Status { get; private set; }

        public int? WinningTeam { get; private set; } // 1 ou 2, definido após RegisterResult
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }


        protected Game() { } // EF Core

    }
}
