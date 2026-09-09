namespace Game.Application.DTOs;

public record SetResultDto(
    int SetNumber,
    int TeamOneGames,
    int TeamTwoGames,
    int? TieBreakTeamOne = null,
    int? TieBreakTeamTwo = null);
