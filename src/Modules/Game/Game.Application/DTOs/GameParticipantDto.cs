namespace Game.Application.DTOs;

public record GameParticipantDto(Guid UserId, int TeamNumber, string Status);
