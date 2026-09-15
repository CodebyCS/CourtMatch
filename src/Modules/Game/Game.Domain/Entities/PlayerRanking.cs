namespace Game.Domain.Entities;

public class PlayerRanking
{
    public Guid UserId { get; private set; }
    public int GamesPlayed { get; private set; }
    public int GamesWon { get; private set; }
    public int SetsWon { get; private set; }
    public int SetsLost { get; private set; }
    public int RankingPoints { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    protected PlayerRanking() { } // EF Core

    /// <summary>
    /// Estatísticas e pontuação de ranking acumuladas por um jogador.
    /// </summary>
    public PlayerRanking(Guid userId)
    {
        UserId = userId;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza as estatísticas do jogador com o resultado de mais um jogo (vitória/derrota e sets).
    /// </summary>
    public void RegisterGameResult(bool won, int setsWon, int setsLost)
    {
        GamesPlayed++;
        if (won) GamesWon++;

        SetsWon += setsWon;
        SetsLost += setsLost;

        RankingPoints += won ? 3 : 1;
        RankingPoints += (setsWon - setsLost);

        LastUpdatedAt = DateTime.UtcNow;
    }
}
