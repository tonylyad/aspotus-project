namespace Aspotus.Catalog.Api.Data.Entities;

public class InventoryReservation
{
    public Guid OrderId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public ICollection<InventoryReservationItem> Items { get; set; } = new List<InventoryReservationItem>();
}
