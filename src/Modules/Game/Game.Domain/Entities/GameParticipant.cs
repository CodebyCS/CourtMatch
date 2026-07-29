using Game.Domain.Entities.Enums;

namespace Game.Domain.Entities;

/// <summary>
/// Representa um jogador convidado/associado a um jogo, numa das duas equipas.
/// </summary>
public class GameParticipant
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Guid UserId { get; private set; }
    public int TeamNumber { get; private set; } // 1 ou 2
    public ParticipantStatus Status { get; private set; }
    public DateTime InvitedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    protected GameParticipant() { } // EF Core

    internal GameParticipant(Guid gameId, Guid userId, int teamNumber)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        UserId = userId;
        TeamNumber = teamNumber;
        Status = ParticipantStatus.Invited;
        InvitedAt = DateTime.UtcNow;
    }

    internal void Confirm()
    {
        Status = ParticipantStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    internal void Decline()
    {
        Status = ParticipantStatus.Declined;
    }
}
