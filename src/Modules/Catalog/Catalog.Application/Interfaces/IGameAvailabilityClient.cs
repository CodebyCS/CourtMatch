namespace Catalog.Application.Interfaces;

public interface IGameAvailabilityClient
{
    Task<bool> IsCourtOccupiedAsync(
        Guid courtId,
        DateTime date,
        TimeSpan startTime,
        CancellationToken ct = default);
}
