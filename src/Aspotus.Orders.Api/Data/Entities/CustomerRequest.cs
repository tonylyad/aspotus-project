namespace Aspotus.Orders.Api.Data.Entities;

public class CustomerRequest
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? Comment { get; set; }
    public string DetailsJson { get; set; } = "{}";
    public string Status { get; set; } = "New";
    public DateTime CreatedAtUtc { get; set; }
}
