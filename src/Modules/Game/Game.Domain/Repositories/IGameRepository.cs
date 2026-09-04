using Game.Domain.Entities;

namespace Game.Domain.Repositories;

public interface IGameRepository
{
    Task<Entities.Game?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Entities.Game?> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Entities.Game>> GetHistoryByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsForBookingAsync(Guid bookingId, CancellationToken ct = default);
    Task AddAsync(Entities.Game game, CancellationToken ct = default);
    Task UpdateAsync(Entities.Game game, CancellationToken ct = default);
}
