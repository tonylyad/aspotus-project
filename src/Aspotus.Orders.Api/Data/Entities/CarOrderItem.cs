namespace Aspotus.Orders.Api.Data.Entities;

/// <summary>
/// Позиция заказа на автомобиль.
/// </summary>
public class CarOrderItem
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
    /// Идентификатор автомобиля из каталога.
    /// </summary>
    public Guid CarId { get; set; }

    /// <summary>
    /// Название марки автомобиля на момент оформления заказа.
    /// </summary>
    public string BrandName { get; set; } = null!;

    /// <summary>
    /// Название модели автомобиля на момент оформления заказа.
    /// </summary>
    public string ModelName { get; set; } = null!;

    /// <summary>
    /// Название поколения автомобиля на момент оформления заказа.
    /// </summary>
    public string GenerationName { get; set; } = null!;

    /// <summary>
    /// Год выпуска автомобиля.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Цена автомобиля на момент оформления заказа.
    /// </summary>
    public decimal Price { get; set; }
}