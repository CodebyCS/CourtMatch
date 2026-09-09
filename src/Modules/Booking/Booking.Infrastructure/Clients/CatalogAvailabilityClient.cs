using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Booking.Application.Interfaces;
using Shared.Contracts.Catalog;

namespace Booking.Infrastructure.Clients;

public class CatalogAvailabilityClient : ICatalogAvailabilityClient
{
    private readonly HttpClient _httpClient;

    public CatalogAvailabilityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AvailabilityResponse?> GetAvailabilityAsync(
        Guid courtId,
        DateTime startTime,
        CancellationToken ct = default)
    {
        var date = startTime.ToString(
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        var time = startTime.ToString(
            "HH:mm:ss",
            CultureInfo.InvariantCulture);

        var url =
            $"api/courts/{courtId}/availability" +
            $"?date={date}&startTime={time}";

        using var response = await _httpClient.GetAsync(url, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AvailabilityResponse>(
            cancellationToken: ct);
    }
}
