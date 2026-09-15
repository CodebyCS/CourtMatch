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
    public Guid CourtId { get; private set; }  // referência ao campo no Catalog.API
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

    public Game(Guid bookingId, Guid courtId, DateTime scheduledAt)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        CourtId = courtId;
        ScheduledAt = scheduledAt;
        Status = GameStatus.PendingConfirmation;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Invites a player to one of the game teams, validating status and team number.
    /// </summary>
    public GameParticipant InvitePlayer(Guid userId, int teamNumber)
    {
        EnsureParticipantsCanChange();
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

        RefreshConfirmationStatus();

        return participant;
    }

    /// <summary>
    /// Confirms the participation of a player who has already been invited and updates the game state.
    /// </summary>
    public void ConfirmParticipant(Guid userId)
    {
        EnsureParticipantsCanChange();

        var participant = GetParticipantOrThrow(userId);

        participant.Confirm();

        RefreshConfirmationStatus();
    }

    /// <summary>
    /// Records a guest player's refusal and updates the game state.
    /// </summary>
    public void DeclineParticipant(Guid userId)
    {
        EnsureParticipantsCanChange();
        var participant = GetParticipantOrThrow(userId);

        participant.Decline();

        RefreshConfirmationStatus();
    }

    /// <summary>
    /// Starts the game, requiring prior confirmation from all players.
    /// </summary>
    public void Start()
    {
        if (Status != GameStatus.Confirmed)
            throw new InvalidOperationException(
                "Só é possível iniciar um jogo que esteja confirmado por todos os jogadores.");

        Status = GameStatus.InProgress;
    }

    /// <summary>
    /// Record the final result (sets) and conclude the match.
    /// </summary>
    public void RegisterResult(IReadOnlyList<GameSet> sets)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Só é possível registar resultados num jogo em curso.");
        }

        if (sets is null || sets.Count == 0)
            throw new ArgumentException(
                "É necessário indicar pelo menos um set.");

        if (sets.Any(set => set is null || set.GameId != Id))
            throw new ArgumentException(
                "Todos os sets devem pertencer a este jogo.");

        var orderedSets = sets
            .OrderBy(set => set.SetNumber)
            .ToList();

        if (!orderedSets.Select(set => set.SetNumber)
                .SequenceEqual(Enumerable.Range(1, orderedSets.Count)))
        {
            throw new ArgumentException(
                "Os sets devem ter números consecutivos, sem repetições.");
        }

        if (orderedSets.Any(set => !GameSet.IsValidScore(
                set.TeamOneGames,
                set.TeamTwoGames,
                set.TieBreakTeamOne,
                set.TieBreakTeamTwo)))
        {
            throw new ArgumentException("Existem pontuações inválidas.");
        }

        var setsTeamOne = orderedSets.Count(
            set => set.WinningTeam() == 1);

        var setsTeamTwo = orderedSets.Count(
            set => set.WinningTeam() == 2);

        if (setsTeamOne == setsTeamTwo)
            throw new ArgumentException(
                "O resultado final deve ter uma equipa vencedora.");

        // Only changes the entity after validations.
        _sets.Clear();
        _sets.AddRange(orderedSets);

        WinningTeam = setsTeamOne > setsTeamTwo ? 1 : 2;
        Status = GameStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the game, provided it has not yet been completed or already cancelled.
    /// </summary>
    public void Cancel()
    {
        if (Status is GameStatus.Completed or GameStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Não é possível cancelar um jogo concluído ou já cancelado.");
        }

        Status = GameStatus.Cancelled;
    }

    /// <summary>
    /// Sets won/lost by a specific player after the result is recorded.
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

    private void EnsureParticipantsCanChange()
    {
        if (Status is not (
            GameStatus.PendingConfirmation or GameStatus.Confirmed))
        {
            throw new InvalidOperationException(
                "Não é possível alterar participantes neste estado.");
        }
    }

    private void RefreshConfirmationStatus()
    {
        var isConfirmed =
            _participants.Count is >= 2 and <= 4 &&
            _participants.Any(p => p.TeamNumber == 1) &&
            _participants.Any(p => p.TeamNumber == 2) &&
            _participants.All(
                p => p.Status == ParticipantStatus.Confirmed);

        Status = isConfirmed
            ? GameStatus.Confirmed
            : GameStatus.PendingConfirmation;
    }
}
