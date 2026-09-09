namespace Game.Application.DTOs;

public record PlayerRankingDto(
    Guid UserId,
    int GamesPlayed,
    int GamesWon,
    double WinRate,
    int SetsWon,
    int SetsLost,
    int RankingPoints);
