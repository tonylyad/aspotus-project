using Aspotus.Catalog.Api.Enums;

namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Запчасть в каталоге.
/// Содержит основную информацию о товаре и его принадлежности
/// к категории и производителю.
/// </summary>
public class Part
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
    /// Может быть пустым.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Цена запчасти.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Количество единиц на складе.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Признак оригинальной запчасти.
    /// true - оригинальная, false - аналог.
    /// </summary>
    public bool IsOriginal { get; set; }

    /// <summary>
    /// Тип состояния запчасти: новая или бывшая в употреблении.
    /// </summary>
    public PartConditionType ConditionType { get; set; }

    /// <summary>
    /// Процент состояния БУ-запчасти.
    /// Для новой запчасти обычно не используется.
    /// </summary>
    public int? ConditionPercent { get; set; }

    /// <summary>
    /// Текстовое описание состояния БУ-запчасти:
    /// дефекты, следы эксплуатации и т.д.
    /// </summary>
    public string? ConditionDescription { get; set; }

    /// <summary>
    /// Пробег автомобиля на момент снятия БУ-запчасти.
    /// Для новой запчасти обычно не используется.
    /// </summary>
    public int? MileageAtRemoval { get; set; }

    /// <summary>
    /// Идентификатор категории запчасти.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Категория запчасти.
    /// </summary>
    public PartCategory Category { get; set; } = null!;

    /// <summary>
    /// Идентификатор производителя запчасти.
    /// </summary>
    public Guid ManufacturerId { get; set; }

    /// <summary>
    /// Производитель запчасти.
    /// </summary>
    public PartManufacturer Manufacturer { get; set; } = null!;

    /// <summary>
    /// Список связей совместимости между запчастью и автомобилями.
    /// </summary>
    public ICollection<PartCompatibility> PartCompatibilities { get; set; } = new List<PartCompatibility>();

    /// <summary>
    /// Список артикулов заменителей для данной запчасти.
    /// </summary>
    public ICollection<PartReplacement> ReplacementArticles { get; set; } = new List<PartReplacement>();
}