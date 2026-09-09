using Game.Application.DTOs;
using Game.Application.Interfaces;
using Game.Domain.Entities;
using Game.Domain.Repositories;
using Shared.Contracts.Exceptions;
using FluentValidation;

namespace Game.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRankingRepository _rankingRepository;

    private readonly IValidator<CreateGameDto> _createValidator;

    private readonly IValidator<RegisterResultDto> _registerResultValidator;

    public GameService(IGameRepository gameRepository, IPlayerRankingRepository rankingRepository,  IValidator<CreateGameDto> createValidator, IValidator<RegisterResultDto> registerResultValidator)
    {
        _gameRepository = gameRepository;
        _rankingRepository = rankingRepository;
        _createValidator = createValidator;
        _registerResultValidator = registerResultValidator;

    }

    public async Task<GameDto> CreateGameAsync(CreateGameDto dto, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
        {
            throw new BadRequestException(
                string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (await _gameRepository.ExistsForBookingAsync(dto.BookingId, ct))
        {
            throw new BadRequestException(
                "Já existe um jogo associado a esta reserva.");
        }

        if (await _gameRepository.IsCourtOccupiedAsync(
                dto.CourtId,
                dto.ScheduledAt,
                ct))
        {
            throw new BadRequestException(
                "The court is already occupied at the selected date and time.");
        }

        var game = new Domain.Entities.Game(dto.BookingId, dto.CourtId, dto.ScheduledAt);

        foreach (var participant in dto.Participants)
        {
            ExecuteDomainOperation(() =>
                game.InvitePlayer(participant.UserId, participant.TeamNumber));
        }

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
        ExecuteDomainOperation(() =>
            game.InvitePlayer(dto.UserId, dto.TeamNumber));

        await _gameRepository.UpdateAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> ConfirmParticipantAsync(Guid gameId, Guid userId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);
        ExecuteDomainOperation(() =>
            game.ConfirmParticipant(userId));

        await _gameRepository.UpdateAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> DeclineParticipantAsync(Guid gameId, Guid userId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);
        ExecuteDomainOperation(() =>
            game.DeclineParticipant(userId));

        await _gameRepository.UpdateAsync(game, ct);
        return ToDto(game);
    }

    public async Task<GameDto> RegisterResultAsync(Guid gameId, RegisterResultDto dto, CancellationToken ct = default)
    {
        var validation = await _registerResultValidator.ValidateAsync(dto, ct);

        if (!validation.IsValid)
        {
            throw new BadRequestException(
                string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
        }
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

        ExecuteDomainOperation(() => game.RegisterResult(sets));
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

    public async Task<bool> IsCourtOccupiedAsync(Guid courtId, DateTime date, TimeSpan startTime, CancellationToken ct = default)
    {
        if (courtId == Guid.Empty)
        throw new BadRequestException("O campo é obrigatório.");

    if (date == default || date.TimeOfDay != TimeSpan.Zero)
        throw new BadRequestException(
            "Indica uma data válida, sem componente de hora.");

    if (startTime < TimeSpan.Zero ||
        startTime >= TimeSpan.FromDays(1))
    {
        throw new BadRequestException(
            "A hora deve estar entre 00:00:00 e 23:59:59.");
    }

    if (startTime.Ticks % TimeSpan.TicksPerSecond != 0)
        throw new BadRequestException(
            "A hora não pode incluir frações de segundo.");

    var scheduledAt = DateTime.SpecifyKind(
        date.Date.Add(startTime),
        DateTimeKind.Utc);

        return await _gameRepository.IsCourtOccupiedAsync(
            courtId,
            scheduledAt,
            ct);
    }

    private async Task<Domain.Entities.Game> GetGameOrThrow(Guid gameId, CancellationToken ct)
    {
        var game = await _gameRepository.GetByIdAsync(gameId, ct);

        if (game is null)
            throw new NotFoundException($"Não foi encontrado nenhum jogo com o Id '{gameId}'.");

        return game;
    }

    public async Task<GameDto> StartGameAsync(Guid gameId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);

        ExecuteDomainOperation(game.Cancel);

        await _gameRepository.UpdateAsync(game, ct);

        return ToDto(game);
    }

    public async Task<GameDto> CancelGameAsync(Guid gameId, CancellationToken ct = default)
    {
        var game = await GetGameOrThrow(gameId, ct);

        ExecuteDomainOperation(game.Cancel);

        await _gameRepository.UpdateAsync(game, ct);

        return ToDto(game);
    }

    private static void ExecuteDomainOperation(Action operation)
    {
        try
        {
            operation();
        }
        catch (ArgumentException ex)
        {
            throw new BadRequestException(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            throw new BadRequestException(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
    }



    private static GameDto ToDto(Domain.Entities.Game game) => new(
        game.Id,
        game.BookingId,
        game.CourtId,
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