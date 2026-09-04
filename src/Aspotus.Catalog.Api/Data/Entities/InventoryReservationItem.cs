namespace Aspotus.Catalog.Api.Data.Entities;

public class InventoryReservationItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public InventoryReservation Reservation { get; set; } = null!;
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
