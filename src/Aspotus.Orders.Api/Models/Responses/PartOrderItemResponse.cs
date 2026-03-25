namespace Aspotus.Orders.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о позиции заказа запчастей.
/// </summary>
public class PartOrderItemResponse
{
    /// <summary>
    /// Уникальный идентификатор позиции заказа.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор запчасти из каталога.
    /// </summary>
    public Guid PartId { get; set; }

    /// <summary>
    /// Название запчасти на момент оформления заказа.
    /// </summary>
    public string PartName { get; set; } = null!;

    /// <summary>
    /// Артикул запчасти на момент оформления заказа.
    /// </summary>
    public string PartArticle { get; set; } = null!;

    /// <summary>
    /// Цена одной единицы запчасти на момент оформления заказа.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Количество единиц запчасти в заказе.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Итоговая стоимость позиции заказа.
    /// </summary>
    public decimal TotalPrice { get; set; }
}