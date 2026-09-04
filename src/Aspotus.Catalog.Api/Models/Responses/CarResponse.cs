namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией об автомобиле.
/// </summary>
public class CarResponse
{
    /// <summary>
    /// Уникальный идентификатор автомобиля.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор марки.
    /// </summary>
    public Guid BrandId { get; set; }

    /// <summary>
    /// Название марки.
    /// </summary>
    public string BrandName { get; set; } = null!;

    /// <summary>
    /// Идентификатор модели.
    /// </summary>
    public Guid ModelId { get; set; }

    /// <summary>
    /// Название модели.
    /// </summary>
    public string ModelName { get; set; } = null!;

    /// <summary>
    /// Идентификатор поколения.
    /// </summary>
    public Guid GenerationId { get; set; }

    /// <summary>
    /// Название поколения.
    /// </summary>
    public string GenerationName { get; set; } = null!;

    /// <summary>
    /// Год выпуска автомобиля.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Пробег автомобиля в километрах.
    /// </summary>
    public int Mileage { get; set; }

    /// <summary>
    /// Цена автомобиля.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Доступен ли автомобиль для нового заказа.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Тип кузова.
    /// </summary>
    public string BodyType { get; set; } = null!;

    /// <summary>
    /// Название комплектации автомобиля.
    /// </summary>
    public string? TrimLevelName { get; set; }

    /// <summary>
    /// Описание комплектации автомобиля.
    /// </summary>
    public string? TrimLevelDescription { get; set; }

    /// <summary>
    /// Объём двигателя.
    /// </summary>
    public decimal EngineVolume { get; set; }

    /// <summary>
    /// Тип топлива.
    /// </summary>
    public string FuelType { get; set; } = null!;

    /// <summary>
    /// Тип коробки передач.
    /// </summary>
    public string TransmissionType { get; set; } = null!;

    /// <summary>
    /// Тип привода.
    /// </summary>
    public string DriveType { get; set; } = null!;

    public List<CatalogImageResponse> Images { get; set; } = new();
}
