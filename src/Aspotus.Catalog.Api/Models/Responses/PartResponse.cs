using Aspotus.Catalog.Api.Enums;

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
    /// Количество, доступное для новых заказов с учётом резервов.
    /// </summary>
    public int AvailableStockQuantity { get; set; }

    public bool IsAvailable => AvailableStockQuantity > 0;

    /// <summary>
    /// Признак оригинальной запчасти.
    /// </summary>
    public bool IsOriginal { get; set; }

    /// <summary>
    /// Тип состояния запчасти.
    /// </summary>
    public PartConditionType ConditionType { get; set; }

    /// <summary>
    /// Процент состояния БУ-запчасти.
    /// </summary>
    public int? ConditionPercent { get; set; }

    /// <summary>
    /// Описание состояния БУ-запчасти.
    /// </summary>
    public string? ConditionDescription { get; set; }

    /// <summary>
    /// Пробег автомобиля на момент снятия БУ-запчасти.
    /// </summary>
    public int? MileageAtRemoval { get; set; }

    /// <summary>
    /// Список артикулов заменителей.
    /// </summary>
    public List<string> ReplacementArticles { get; set; } = new();

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

    public List<CatalogImageResponse> Images { get; set; } = new();
}
