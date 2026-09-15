using Game.Application.DTOs;
using Game.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Contracts.Game;

namespace Game.API.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService) => _gameService = gameService;

    /// <summary>
    /// Creates a game associated with a booking (typically called by Ordering.API/Booking.API).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<GameDto>> Create([FromBody] CreateGameDto dto, CancellationToken ct)
    {
        var game = await _gameService.CreateGameAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { gameId = game.Id }, game);
    }

    /// <summary>
    /// Retrieves the details of a game by its identifier.
    /// </summary>
    [HttpGet("{gameId:guid}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GameDto>> GetById(Guid gameId, CancellationToken ct)
    {
        var game = await _gameService.GetByIdAsync(gameId, ct);
        return Ok(game);
    }

    /// <summary>
    /// Invites a player to one of the game's teams.
    /// </summary>
    [HttpPost("{gameId:guid}/invite")]
    public async Task<ActionResult<GameDto>> Invite(Guid gameId, [FromBody] InvitePlayerDto dto, CancellationToken ct)
    {
        var game = await _gameService.InvitePlayerAsync(gameId, dto, ct);
        return Ok(game);
    }

    /// <summary>
    /// A player confirms their participation.
    /// </summary>

    [HttpPost("{gameId:guid}/players/{userId:guid}/confirm")]
    public async Task<ActionResult<GameDto>> Confirm(Guid gameId, Guid userId, CancellationToken ct)
    {
        var game = await _gameService.ConfirmParticipantAsync(gameId, userId, ct);
        return Ok(game);
    }

    /// <summary>
    /// A player declines the invitation.
    /// </summary>
    [HttpPost("{gameId:guid}/players/{userId:guid}/decline")]
    public async Task<ActionResult<GameDto>> Decline(Guid gameId, Guid userId, CancellationToken ct)
    {
        var game = await _gameService.DeclineParticipantAsync(gameId, userId, ct);
        return Ok(game);
    }

    /// <summary>
    /// Records the final result (sets) of the match and updates statistics/rankings.
    /// </summary>
    [HttpPost("{gameId:guid}/result")]
    public async Task<ActionResult<GameDto>> RegisterResult(Guid gameId, [FromBody] RegisterResultDto dto, CancellationToken ct)
    {
        var game = await _gameService.RegisterResultAsync(gameId, dto, ct);
        return Ok(game);
    }

    /// <summary>
    /// History of games played by a user.
    /// </summary>
    [HttpGet("history/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<GameDto>>> GetHistory(Guid userId, CancellationToken ct)
    {
        var history = await _gameService.GetHistoryByUserIdAsync(userId, ct);
        return Ok(history);
    }

    /// <summary>
    /// Checks if a field is occupied at a specific start date/time.
    /// </summary>

    [HttpGet("courts/{courtId:guid}/occupied")]
[ProducesResponseType(
    typeof(CourtOccupiedResponse),
    StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<CourtOccupiedResponse>> IsCourtOccupied(
    Guid courtId,
    [FromQuery, BindRequired] DateTime date,
    [FromQuery, BindRequired] TimeSpan startTime,
    CancellationToken ct)
    {
        var isOccupied = await _gameService.IsCourtOccupiedAsync(
            courtId,
            date,
            startTime,
            ct);

        return Ok(new CourtOccupiedResponse
        {
            IsOccupied = isOccupied
        });
    }

    /// <summary>
    /// Starts a previously created game.
    /// </summary>
    [HttpPost("{gameId:guid}/start")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> Start(
        Guid gameId,
        CancellationToken ct)
    {
        var game = await _gameService.StartGameAsync(gameId, ct);
        return Ok(game);
    }

    // POST /api/games/{gameId}/cancel

    /// <summary>
    /// Cancels a game.
    /// </summary>

    [HttpPost("{gameId:guid}/cancel")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> Cancel(
        Guid gameId,
        CancellationToken ct)
    {
        var game = await _gameService.CancelGameAsync(gameId, ct);

        return Ok(game);
}
}
