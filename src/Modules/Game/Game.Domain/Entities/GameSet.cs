namespace Game.Domain.Entities;

/// <summary>
/// Result of an individual set within a match (with tie-break support).
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

    /// <summary>
    /// Creates a set, validating the set number and the score (including the tie-break, when applicable).
    /// </summary>
    public GameSet(Guid gameId, int setNumber, int teamOneGames, int teamTwoGames,
        int? tieBreakTeamOne = null, int? tieBreakTeamTwo = null)
    {
        if (gameId == Guid.Empty)
            throw new ArgumentException("O jogo é obrigatório.");

        if (setNumber <= 0)
            throw new ArgumentException(
                "O número do set deve ser positivo.");

        if (!IsValidScore(
                teamOneGames,
                teamTwoGames,
                tieBreakTeamOne,
                tieBreakTeamTwo))
        {
            throw new ArgumentException(
                "A pontuação do set ou do tie-break é inválida.");
        }

        Id = Guid.NewGuid();
        GameId = gameId;
        SetNumber = setNumber;
        TeamOneGames = teamOneGames;
        TeamTwoGames = teamTwoGames;
        TieBreakTeamOne = tieBreakTeamOne;
        TieBreakTeamTwo = tieBreakTeamTwo;
    }

    /// <summary>
    /// Checks whether a score (and the corresponding tie-break) is valid for a set.
    /// </summary>  
    public static bool IsValidScore(
        int teamOneGames,
        int teamTwoGames,
        int? tieBreakTeamOne,
        int? tieBreakTeamTwo)
    {
        if (teamOneGames < 0 ||
            teamTwoGames < 0 ||
            teamOneGames == teamTwoGames)
        {
            return false;
        }

        // The two tie-break values ​​must be sent together.
        if (tieBreakTeamOne.HasValue != tieBreakTeamTwo.HasValue)
            return false;

        if (!tieBreakTeamOne.HasValue)
            return true;

        var first = tieBreakTeamOne.Value;
        var second = tieBreakTeamTwo!.Value;

        if (first < 0 || second < 0 || first == second)
            return false;

        // The winner of the tie-break must be the winner of the set.
        return (first > second) == (teamOneGames > teamTwoGames);
    }
    /// <summary>
    /// Returns the team that won the set (1 or 2).
    /// </summary>
    public int WinningTeam()
    {
        if (TeamOneGames == TeamTwoGames)
            throw new InvalidOperationException(
                "Um set concluído não pode terminar empatado.");

        return TeamOneGames > TeamTwoGames ? 1 : 2;
    }
}
