using Game.Domain.Entities;
using Game.Domain.Entities.Enums;


namespace Game.Domain.Entities;

/// <summary>
/// Aggregate root do módulo Games. Representa a partida associada a uma reserva
/// (BookingId), com os jogadores convidados/confirmados e o resultado final.
/// </summary>
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
        if (Status is GameStatus.Completed or GameStatus.Cancelled)
            throw new InvalidOperationException(
                $"Não é possível convidar jogadores para um jogo com estado '{Status}'.");

        if (teamNumber is not (1 or 2))
            throw new ArgumentException(
                "O número da equipa tem de ser 1 ou 2.",
                nameof(teamNumber));

        if (_participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException(
                $"O jogador com o Id '{userId}' já participa neste jogo.");

        if (_participants.Count(p => p.TeamNumber == teamNumber) >= 2)
            throw new InvalidOperationException(
                $"A equipa {teamNumber} já tem o número máximo de jogadores (2).");

        var participant = new GameParticipant(Id, userId, teamNumber);
        _participants.Add(participant);

        return participant;
    }

    public void ConfirmParticipant(Guid userId)
    {
        var participant = GetParticipantOrThrow(userId);

        participant.Confirm();

        var minimoJogadores = 2; // permite singles (1v1); para pares, o serviço pode validar 4

        if (_participants.Count >= minimoJogadores &&
            _participants.All(p => p.Status == ParticipantStatus.Confirmed))
        {
            Status = GameStatus.Confirmed;
        }
    }

    public void DeclineParticipant(Guid userId)
    {
        var participant = GetParticipantOrThrow(userId);

        participant.Decline();
    }

    public void Start()
    {
        if (Status != GameStatus.Confirmed)
            throw new InvalidOperationException(
                "Só é possível iniciar um jogo que esteja confirmado por todos os jogadores.");

        Status = GameStatus.InProgress;
    }

    /// <summary>
    /// Regista o resultado final (sets) e conclui o jogo.
    /// </summary>
    public void RegisterResult(IReadOnlyList<GameSet> sets)
    {
        if (Status == GameStatus.Cancelled)
            throw new InvalidOperationException(
                "Não é possível registar resultado num jogo cancelado.");

        if (sets == null)
            throw new ArgumentNullException(nameof(sets));

        if (sets.Count == 0)
            throw new ArgumentException(
                "É necessário indicar pelo menos um set.",
                nameof(sets));

        _sets.Clear();
        _sets.AddRange(sets);

        var setsTeam1 = sets.Count(s => s.WinningTeam() == 1);
        var setsTeam2 = sets.Count(s => s.WinningTeam() == 2);

        WinningTeam = setsTeam1 > setsTeam2 ? 1 : 2;
        Status = GameStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == GameStatus.Completed)
            throw new InvalidOperationException(
                "Não é possível cancelar um jogo já concluído.");

        Status = GameStatus.Cancelled;
    }

    /// <summary>
    /// Sets ganhos/perdidos por um jogador específico após resultado registado.
    /// </summary>
    public (int setsWon, int setsLost) GetSetBalanceForUser(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId);

        if (participant is null || _sets.Count == 0)
            return (0, 0);

        var setsWon = _sets.Count(s => s.WinningTeam() == participant.TeamNumber);
        var setsLost = _sets.Count - setsWon;

        return (setsWon, setsLost);
    }

    private GameParticipant GetParticipantOrThrow(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId);

        if (participant is null)
            throw new KeyNotFoundException(
                $"Não foi encontrado nenhum participante com o Id '{userId}'.");

        return participant;
    }
}
