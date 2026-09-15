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
    /// Statistics and ranking points accumulated by a player.
    /// </summary>
    public PlayerRanking(Guid userId)
    {
        UserId = userId;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the player's statistics with the result of another match (win/loss and sets).
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
