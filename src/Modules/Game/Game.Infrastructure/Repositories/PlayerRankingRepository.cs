using Game.Domain.Entities;
using Game.Domain.Repositories;
using Game.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Game.Infrastructure.Repositories;

public class PlayerRankingRepository : IPlayerRankingRepository
{
    private readonly GameDbContext _context;

    public PlayerRankingRepository(GameDbContext context) => _context = context;

    public async Task<PlayerRanking?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.PlayerRankings.FirstOrDefaultAsync(r => r.UserId == userId, ct);

    public async Task<IReadOnlyList<PlayerRanking>> GetTopAsync(int count, CancellationToken ct = default) =>
        await _context.PlayerRankings
            .OrderByDescending(r => r.RankingPoints)
            .Take(count)
            .ToListAsync(ct);

    public async Task UpsertAsync(PlayerRanking ranking, CancellationToken ct = default)
    {
        var exists = await _context.PlayerRankings.AnyAsync(r => r.UserId == ranking.UserId, ct);
        if (exists)
            _context.PlayerRankings.Update(ranking);
        else
            await _context.PlayerRankings.AddAsync(ranking, ct);

        await _context.SaveChangesAsync(ct);
    }
}
