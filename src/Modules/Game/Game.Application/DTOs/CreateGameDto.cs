namespace Game.Application.DTOs;

public record CreateGameDto(
    Guid BookingId,
    Guid FacilityId,
    DateTime ScheduledAt,
    List<InitialParticipantDto> Participants);

public record InitialParticipantDto(Guid UserId, int TeamNumber);
