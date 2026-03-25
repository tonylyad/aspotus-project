namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о запчасти.
/// </summary>
public class PartResponse
{
    /// <summary>
    /// Уникальный идентификатор запчасти.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название запчасти.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Артикул запчасти.
    /// </summary>
    public string Article { get; set; } = null!;

    /// <summary>
    /// Описание запчасти.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Цена запчасти.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Количество на складе.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Признак оригинальной запчасти.
    /// </summary>
    public bool IsOriginal { get; set; }

    /// <summary>
    /// Идентификатор категории.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Название категории.
    /// </summary>
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// Идентификатор производителя.
    /// </summary>
    public Guid ManufacturerId { get; set; }

    /// <summary>
    /// Название производителя.
    /// </summary>
    public string ManufacturerName { get; set; } = null!;
}