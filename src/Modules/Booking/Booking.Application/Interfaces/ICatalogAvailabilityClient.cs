using Shared.Contracts.Catalog;

namespace Booking.Application.Interfaces;

public interface ICatalogAvailabilityClient
{
    Task<AvailabilityResponse?> GetAvailabilityAsync(
        Guid courtId,
        DateTime startTime,
        CancellationToken ct = default);
}
