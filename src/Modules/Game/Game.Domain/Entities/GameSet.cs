namespace Game.Domain.Entities;

/// <summary>
/// Resultado de um set individual dentro de um jogo (com suporte a tie-break).
/// </summary>
public class GameSet
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public int SetNumber { get; private set; }
    public int TeamOneGames { get; private set; }
    public int TeamTwoGames { get; private set; }
    public int? TieBreakTeamOne { get; private set; }
    public int? TieBreakTeamTwo { get; private set; }

    protected GameSet() { } // EF Core

    public GameSet(Guid gameId, int setNumber, int teamOneGames, int teamTwoGames,
        int? tieBreakTeamOne = null, int? tieBreakTeamTwo = null)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        SetNumber = setNumber;
        TeamOneGames = teamOneGames;
        TeamTwoGames = teamTwoGames;
        TieBreakTeamOne = tieBreakTeamOne;
        TieBreakTeamTwo = tieBreakTeamTwo;
    }

    public int WinningTeam() => TeamOneGames > TeamTwoGames ? 1 : 2;
}
