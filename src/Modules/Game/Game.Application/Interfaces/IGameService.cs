using Games.Application.DTOs;

namespace Games.Application.Interfaces;

public interface IGameService
{
    Task<GameDto> CreateGameAsync(CreateGameDto dto, CancellationToken ct = default);
    Task<GameDto> GetByIdAsync(Guid gameId, CancellationToken ct = default);
    Task<GameDto> InvitePlayerAsync(Guid gameId, InvitePlayerDto dto, CancellationToken ct = default);
    Task<GameDto> ConfirmParticipantAsync(Guid gameId, Guid userId, CancellationToken ct = default);
    Task<GameDto> DeclineParticipantAsync(Guid gameId, Guid userId, CancellationToken ct = default);
    Task<GameDto> RegisterResultAsync(Guid gameId, RegisterResultDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<GameDto>> GetHistoryByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<PlayerRankingDto>> GetRankingAsync(int top, CancellationToken ct = default);
}
