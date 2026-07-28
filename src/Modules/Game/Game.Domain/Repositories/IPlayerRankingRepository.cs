using Game.Domain;

namespace Game.Domain.Repositories;

public interface IPlayerRankingRepository
{
    Task<Domain.Entities.PlayerRanking?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Domain.Entities.PlayerRanking>> GetTopAsync(int count, CancellationToken ct = default);
    Task UpsertAsync(Domain.Entities.PlayerRanking ranking, CancellationToken ct = default);
}
