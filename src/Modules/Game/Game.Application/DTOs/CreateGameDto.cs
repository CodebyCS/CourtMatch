namespace Game.Application.DTOs;

public record CreateGameDto(
    Guid BookingId,
    Guid FacilityId,
    DateTime ScheduledAt,
    List<InitialParticipantDto> Participants,
    string? Name = null,
    string Format = "Doubles");

public record InitialParticipantDto(Guid UserId, int TeamNumber);
