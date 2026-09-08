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

        // Os dois valores do tie-break devem ser enviados em conjunto.
        if (tieBreakTeamOne.HasValue != tieBreakTeamTwo.HasValue)
            return false;

        if (!tieBreakTeamOne.HasValue)
            return true;

        var first = tieBreakTeamOne.Value;
        var second = tieBreakTeamTwo!.Value;

        if (first < 0 || second < 0 || first == second)
            return false;

        // O vencedor do tie-break deve ser o vencedor do set.
        return (first > second) == (teamOneGames > teamTwoGames);
    }

    public int WinningTeam()
    {
        if (TeamOneGames == TeamTwoGames)
            throw new InvalidOperationException(
                "Um set concluído não pode terminar empatado.");

        return TeamOneGames > TeamTwoGames ? 1 : 2;
    }
}
