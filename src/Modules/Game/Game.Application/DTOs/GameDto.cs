namespace Game.Application.DTOs;

public record GameDto(
    Guid Id,
    Guid BookingId,
    Guid CourtId,
    DateTime ScheduledAt,
    string Status,
    int? WinningTeam,
    List<GameParticipantDto> Participants,
    List<SetResultDto> Sets);
