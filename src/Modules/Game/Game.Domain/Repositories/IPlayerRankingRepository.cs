using Game.Domain;

namespace Game.Domain.Repositories;

public interface IPlayerRankingRepository
{
    Task<PlayerRanking?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<PlayerRanking>> GetTopAsync(int count, CancellationToken ct = default);
    Task UpsertAsync(PlayerRanking ranking, CancellationToken ct = default);
}
