namespace Aspotus.Orders.Api.Clients;

public interface ICatalogInventoryClient
{
    Task<CatalogReservationResponse> ReserveAsync(
        Guid orderId,
        Guid? userId,
        IReadOnlyCollection<CatalogReservationItemRequest> items,
        CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken = default);
}

public record CatalogReservationItemRequest(string ProductType, Guid ProductId, int Quantity);

public class CatalogReservationResponse
{
    public Guid OrderId { get; set; }
    public List<CatalogReservationItemResponse> Items { get; set; } = new();
}

public class CatalogReservationItemResponse
{
    public string ProductType { get; set; } = null!;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Name { get; set; } = null!;
    public string? Article { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public string? GenerationName { get; set; }
    public int? Year { get; set; }
}
