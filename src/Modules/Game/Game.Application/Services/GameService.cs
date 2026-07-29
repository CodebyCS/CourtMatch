using Game.Application.DTOs;
using Game.Application.Interfaces;
using Game.Domain.Entities;
using Game.Domain.Repositories;

namespace Game.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRankingRepository _rankingRepository;

    public GameService(IGameRepository gameRepository, IPlayerRankingRepository rankingRepository)
    {
        _gameRepository = gameRepository;
        _rankingRepository = rankingRepository;
    }

    public async Task<GameDto> CreateGameAsync(CreateGameDto dto, CancellationToken ct = default)
    {
        if (await _gameRepository.ExistsForBookingAsync(dto.BookingId, ct))
            throw new InvalidOperationException($"Já existe um jogo associado à reserva '{dto.BookingId}'.");

        var game = new Domain.Entities.Game(dto.BookingId, dto.FacilityId, dto.ScheduledAt);

        foreach (var participant in dto.Participants)
            game.InvitePlayer(participant.UserId, participant.TeamNumber);

        await _gameRepository.AddAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> GetByIdAsync(Guid gameId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);
        return ToDto(game);
    }

    public async Task<GameDto> InvitePlayerAsync(Guid gameId, InvitePlayerDto dto, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);
        game.InvitePlayer(dto.UserId, dto.TeamNumber);

        await _gameRepository.UpdateAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> ConfirmParticipantAsync(Guid gameId, Guid userId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);
        game.ConfirmParticipant(userId);

        await _gameRepository.UpdateAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> DeclineParticipantAsync(Guid gameId, Guid userId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);
        game.DeclineParticipant(userId);

        await _gameRepository.UpdateAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> RegisterResultAsync(Guid gameId, RegisterResultDto dto, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);

        var sets = dto.Sets
            .Select(s => new GameSet(
                gameId,
                s.SetNumber,
                s.TeamOneGames,
                s.TeamTwoGames,
                s.TieBreakTeamOne,
                s.TieBreakTeamTwo))
            .ToList();

        game.RegisterResult(sets);
        await _gameRepository.UpdateAsync(game, ct);

        foreach (var participant in game.Participants)
        {
            var (setsWon, setsLost) = game.GetSetBalanceForUser(participant.UserId);
            var won = participant.TeamNumber == game.WinningTeam;

            var ranking = await _rankingRepository.GetByUserIdAsync(participant.UserId, ct)
                ?? new PlayerRanking(participant.UserId);

            ranking.RegisterGameResult(won, setsWon, setsLost);
            await _rankingRepository.UpsertAsync(ranking, ct);
        }

        return ToDto(game);
    }

    public async Task<IReadOnlyList<GameDto>> GetHistoryByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var games = await _gameRepository.GetHistoryByUserIdAsync(userId, ct);
        return games.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<PlayerRankingDto>> GetRankingAsync(int top, CancellationToken ct = default)
    {
        var rankings = await _rankingRepository.GetTopAsync(top, ct);

        return rankings
            .OrderByDescending(r => r.RankingPoints)
            .Select(r => new PlayerRankingDto(
                r.UserId,
                r.GamesPlayed,
                r.GamesWon,
                r.GamesPlayed == 0 ? 0 : Math.Round((double)r.GamesWon / r.GamesPlayed * 100, 1),
                r.SetsWon,
                r.SetsLost,
                r.RankingPoints))
            .ToList();
    }

    private async Task<Domain.Entities.Game> GetGameOrThrow(Guid gameId, CancellationToken ct)
    {
        var game = await _gameRepository.GetByIdAsync(gameId, ct);

        if (game is null)
            throw new KeyNotFoundException($"Não foi encontrado nenhum jogo com o Id '{gameId}'.");

        return game;
    }

    private static GameDto ToDto(Domain.Entities.Game game) => new(
        game.Id,
        game.BookingId,
        game.FacilityId,
        game.ScheduledAt,
        game.Status.ToString(),
        game.WinningTeam,
        game.Participants
            .Select(p => new GameParticipantDto(
                p.UserId,
                p.TeamNumber,
                p.Status.ToString()))
            .ToList(),
        game.Sets
            .Select(s => new SetResultDto(
                s.SetNumber,
                s.TeamOneGames,
                s.TeamTwoGames,
                s.TieBreakTeamOne,
                s.TieBreakTeamTwo))
            .ToList());
}