using Game.Domain.Entities;
using Game.Domain.Repositories;
using Game.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Game.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext _context;

    public GameRepository(GameDbContext context) => _context = context;

    public async Task<Domain.Entities.Game?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.Sets)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IReadOnlyList<Domain.Entities.Game>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default) =>
        await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.Sets)
            .Where(g => g.BookingId == bookingId)
            .OrderByDescending(g => g.ScheduledAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Domain.Entities.Game>> GetHistoryByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.Sets)
            .Where(g => g.Participants.Any(p => p.UserId == userId))
            .OrderByDescending(g => g.ScheduledAt)
            .ToListAsync(ct);

    public async Task AddAsync(Domain.Entities.Game game, CancellationToken ct = default)
    {
        await _context.Games.AddAsync(game, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Domain.Entities.Game game, CancellationToken ct = default)
    {
        // Domain methods (InvitePlayer, RegisterResult) construct child entities with their
        // Guid Id already assigned. EF's change tracker treats a newly-discovered entity with
        // a non-default key as "existing" (Modified) rather than new (Added) - including via
        // ChangeTracker.Entries<T>(), which runs its own DetectChanges pass and applies that
        // same heuristic before we get to check anything. So instead of asking the tracker
        // what it thinks exists, ask the database directly (untracked, so it can't disturb
        // tracking state) and mark whatever it doesn't have yet as Added before saving.
        var existingParticipantIds = await _context.GameParticipants.AsNoTracking()
            .Where(p => p.GameId == game.Id).Select(p => p.Id).ToListAsync(ct);
        var existingParticipantIdSet = existingParticipantIds.ToHashSet();
        foreach (var participant in game.Participants)
            if (!existingParticipantIdSet.Contains(participant.Id))
                _context.Entry(participant).State = EntityState.Added;

        var existingSetIds = await _context.GameSets.AsNoTracking()
            .Where(s => s.GameId == game.Id).Select(s => s.Id).ToListAsync(ct);
        var existingSetIdSet = existingSetIds.ToHashSet();
        foreach (var set in game.Sets)
            if (!existingSetIdSet.Contains(set.Id))
                _context.Entry(set).State = EntityState.Added;

        await _context.SaveChangesAsync(ct);
    }
}
