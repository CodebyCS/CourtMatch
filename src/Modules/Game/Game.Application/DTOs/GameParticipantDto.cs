namespace Games.Application.DTOs;

public record GameParticipantDto(Guid UserId, int TeamNumber, string Status);
