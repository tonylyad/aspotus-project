using System.ComponentModel.DataAnnotations;
using Aspotus.Orders.Api.Enums;

namespace Aspotus.Orders.Api.Models.Requests;

public class UpdateOrderStatusRequest
{
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }
}
