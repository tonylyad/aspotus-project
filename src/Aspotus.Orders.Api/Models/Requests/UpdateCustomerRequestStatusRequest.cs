using System.ComponentModel.DataAnnotations;

namespace Aspotus.Orders.Api.Models.Requests;

public class UpdateCustomerRequestStatusRequest
{
    [Required]
    [RegularExpression("^(New|Processing|Completed|Cancelled)$")]
    public string Status { get; set; } = null!;
}
