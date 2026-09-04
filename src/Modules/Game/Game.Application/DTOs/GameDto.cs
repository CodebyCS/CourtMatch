namespace Game.Application.DTOs;

public record GameDto(
    Guid Id,
    Guid BookingId,
    Guid FacilityId,
    DateTime ScheduledAt,
    string Status,
    int? WinningTeam,
    List<GameParticipantDto> Participants,
    List<SetResultDto> Sets);
