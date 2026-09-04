using System.ComponentModel.DataAnnotations;

namespace Aspotus.Orders.Api.Models.Requests;

public class CreateCustomerRequest
{
    [Required]
    [RegularExpression("^(auto|spare)$")]
    public string Type { get; set; } = null!;

    [Required, StringLength(200)]
    public string CustomerName { get; set; } = null!;

    [Required, EmailAddress, StringLength(320)]
    public string CustomerEmail { get; set; } = null!;

    [Required, StringLength(50)]
    public string CustomerPhone { get; set; } = null!;

    [StringLength(2000)]
    public string? Comment { get; set; }

    public Dictionary<string, string?> Details { get; set; } = new();
}
