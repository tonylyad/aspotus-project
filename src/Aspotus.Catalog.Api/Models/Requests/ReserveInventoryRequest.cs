using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

public class ReserveInventoryRequest
{
    [Required]
    public Guid OrderId { get; set; }

    public Guid? UserId { get; set; }

    [Required, MinLength(1)]
    public List<ReserveInventoryItemRequest> Items { get; set; } = new();
}

public class ReserveInventoryItemRequest
{
    [Required]
    public string ProductType { get; set; } = null!;

    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
