namespace Game.Application.DTOs;

public record CreateGameDto(
    Guid BookingId,
    Guid CourtId,
    DateTime ScheduledAt,
    List<InitialParticipantDto> Participants);

public record InitialParticipantDto(Guid UserId, int TeamNumber);
