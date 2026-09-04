using System.ComponentModel.DataAnnotations;

namespace Aspotus.Orders.Api.Models.Requests;

/// <summary>
/// Позиция запроса на создание заказа запчастей.
/// </summary>
public class CreatePartOrderItemRequest
{
    /// <summary>
    /// Идентификатор запчасти из каталога.
    /// </summary>
    [Required(ErrorMessage = "Идентификатор запчасти обязателен.")]
    public Guid PartId { get; set; }

    /// <summary>
    /// Название запчасти на момент оформления заказа.
    /// </summary>
    public string? PartName { get; set; }

    /// <summary>
    /// Артикул запчасти на момент оформления заказа.
    /// </summary>
    public string? PartArticle { get; set; }

    /// <summary>
    /// Цена одной единицы запчасти на момент оформления заказа.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Количество единиц запчасти в заказе.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не меньше 1.")]
    public int Quantity { get; set; }
}
