namespace Aspotus.Orders.Api.Data.Entities;

/// <summary>
/// Позиция заказа на запчасть.
/// </summary>
public class PartOrderItem
{
    /// <summary>
    /// Уникальный идентификатор позиции заказа.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор заказа.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Заказ, к которому относится позиция.
    /// </summary>
    public Order Order { get; set; } = null!;

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
    /// Цена одной единицы на момент оформления заказа.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Количество единиц.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Итоговая стоимость позиции.
    /// </summary>
    public decimal TotalPrice { get; set; }
}