using System.Globalization;
using System.Net.Http.Json;
using Catalog.Application.Interfaces;
using Shared.Contracts.Game;

namespace Catalog.Infrastructure.Clients;

public class GameAvailabilityClient : IGameAvailabilityClient
{
    private readonly HttpClient _httpClient;

    public GameAvailabilityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsCourtOccupiedAsync(
        Guid courtId,
        DateTime date,
        TimeSpan startTime,
        CancellationToken ct = default)
    {
        var dateValue = date.ToString(
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        var timeValue = startTime.ToString(
            @"hh\:mm\:ss",
            CultureInfo.InvariantCulture);

        var url =
            $"api/games/courts/{courtId}/occupied" +
            $"?date={dateValue}&startTime={timeValue}";

        var response = await _httpClient
            .GetFromJsonAsync<CourtOccupiedResponse>(url, ct);

        return response?.IsOccupied
            ?? throw new HttpRequestException(
                "A Game API devolveu uma resposta inválida.");
    }
}