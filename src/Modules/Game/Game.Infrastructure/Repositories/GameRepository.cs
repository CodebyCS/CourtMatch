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

    public async Task<Domain.Entities.Game?> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default) =>
        await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.Sets)
            .FirstOrDefaultAsync(g => g.BookingId == bookingId, ct);

    public async Task<IReadOnlyList<Domain.Entities.Game>> GetHistoryByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.Sets)
            .Where(g => g.Participants.Any(p => p.UserId == userId))
            .OrderByDescending(g => g.ScheduledAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsForBookingAsync(Guid bookingId, CancellationToken ct = default) =>
        await _context.Games.AnyAsync(g => g.BookingId == bookingId, ct);

    public async Task AddAsync(Domain.Entities.Game game, CancellationToken ct = default)
    {
        await _context.Games.AddAsync(game, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Domain.Entities.Game game, CancellationToken ct = default)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync(ct);
    }
}
