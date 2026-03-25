namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о поколении автомобиля.
/// </summary>
public class CarGenerationResponse
{
    /// <summary>
    /// Уникальный идентификатор поколения.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название поколения.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Год начала выпуска.
    /// </summary>
    public int YearFrom { get; set; }

    /// <summary>
    /// Год окончания выпуска.
    /// </summary>
    public int? YearTo { get; set; }

    /// <summary>
    /// Идентификатор модели автомобиля.
    /// </summary>
    public Guid ModelId { get; set; }

    /// <summary>
    /// Название модели автомобиля.
    /// </summary>
    public string ModelName { get; set; } = null!;
}