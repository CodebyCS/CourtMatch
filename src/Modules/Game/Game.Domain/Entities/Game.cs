using Game.Domain.Entities.Enums;

namespace Game.Domain.Entities
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

        private readonly List<GameParticipant> _participants = new();
        public IReadOnlyCollection<GameParticipant> Participants => _participants.AsReadOnly();

        private readonly List<GameSet> _sets = new();
        public IReadOnlyCollection<GameSet> Sets => _sets.AsReadOnly();

        protected Game() { } // EF Core

        public Game(Guid bookingId, Guid facilityId, DateTime scheduledAt)
        {
            Id = Guid.NewGuid();
            BookingId = bookingId;
            FacilityId = facilityId;
            ScheduledAt = scheduledAt;
            Status = GameStatus.PendingConfirmation;
            CreatedAt = DateTime.UtcNow;
        }

        public GameParticipant InvitePlayer(Guid userId, int teamNumber)
        {
           
            var participant = new GameParticipant(Id, userId, teamNumber);
            _participants.Add(participant);
            return participant;
        }

        public void ConfirmParticipant(Guid userId)
        {
         

        //    var minimoJogadores = 2; // permite 1v1; para pares, o serviço pode validar 4
        //    if (_participants.Count >= minimoJogadores && _participants.All(p => p.Status == ParticipantStatus.Confirmed))
        //        Status = GameStatus.Confirmed;
        }

        public void DeclineParticipant(Guid userId)
        {
           
        }

        public void Start()
        {
            if (Status != GameStatus.Confirmed)
                //exception


            Status = GameStatus.InProgress;
        }

        /// <summary>
        /// Regista o resultado final (sets) e conclui o jogo.
        /// </summary>
        public void RegisterResult(IReadOnlyList<GameSet> sets)
        {
           //validar
           
            //_sets.Clear();
            //_sets.AddRange(sets);

            //var setsTeam1 = sets.Count(s => s.WinningTeam() == 1);
            //var setsTeam2 = sets.Count(s => s.WinningTeam() == 2);

            //WinningTeam = setsTeam1 > setsTeam2 ? 1 : 2;
            //Status = GameStatus.Completed;
            //CompletedAt = DateTime.UtcNow;
        }


        /// <summary>Sets ganhos/perdidos por um jogador específico (após resultado registado).</summary>
        public (int setsWon, int setsLost) GetSetBalanceForUser(Guid userId)
        {
            var participant = _participants.FirstOrDefault(p => p.UserId == userId);
            if (participant is null || _sets.Count == 0)
                return (0, 0);

            var setsWon = _sets.Count(s => s.WinningTeam() == participant.TeamNumber);
            var setsLost = _sets.Count - setsWon;
            return (setsWon, setsLost);
        }
    }
}
