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
    [Required(ErrorMessage = "Название запчасти обязательно для заполнения.")]
    public string PartName { get; set; } = null!;

    /// <summary>
    /// Артикул запчасти на момент оформления заказа.
    /// </summary>
    [Required(ErrorMessage = "Артикул запчасти обязателен для заполнения.")]
    public string PartArticle { get; set; } = null!;

    /// <summary>
    /// Цена одной единицы запчасти на момент оформления заказа.
    /// </summary>
    [Range(0.01, 1000000000, ErrorMessage = "Цена запчасти должна быть больше 0.")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Количество единиц запчасти в заказе.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не меньше 1.")]
    public int Quantity { get; set; }
}