using Game.Application.DTOs;
using Game.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Game.API.Controllers;

[ApiController]
[Route("api/ranking")]
public class RankingController : ControllerBase
{
    private readonly IGameService _gameService;

    public RankingController(IGameService gameService) => _gameService = gameService;

    //Ranking dos jogadores por pontuação (top N, por omissão 20).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlayerRankingDto>>> GetRanking([FromQuery] int top = 20, CancellationToken ct = default)
    {
        var ranking = await _gameService.GetRankingAsync(top, ct);
        return Ok(ranking);
    }
}
