using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Aspotus.Orders.Api.Exceptions;

namespace Aspotus.Orders.Api.Clients;

public class CatalogInventoryClient : ICatalogInventoryClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CatalogInventoryClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<CatalogReservationResponse> ReserveAsync(
        Guid orderId,
        Guid? userId,
        IReadOnlyCollection<CatalogReservationItemRequest> items,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/inventory-reservations")
        {
            Content = JsonContent.Create(new { orderId, userId, items })
        };
        AddApiKey(request);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ValidationException(await ReadErrorAsync(response, cancellationToken));
        }

        return await response.Content.ReadFromJsonAsync<CatalogReservationResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Catalog API вернул пустой ответ резервирования.");
    }

    public async Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/inventory-reservations/{orderId}");
        AddApiKey(request);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private void AddApiKey(HttpRequestMessage request)
    {
        request.Headers.Add("X-Internal-Api-Key", _configuration["Catalog:InternalApiKey"]);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            if (json.TryGetProperty("message", out var message)) return message.GetString() ?? "Товар недоступен.";
        }
        catch (JsonException)
        {
            // Возвращаем стабильное сообщение ниже.
        }

        return response.StatusCode == HttpStatusCode.Conflict
            ? "Товар уже зарезервирован другим покупателем."
            : "Не удалось проверить наличие товара.";
    }
}
