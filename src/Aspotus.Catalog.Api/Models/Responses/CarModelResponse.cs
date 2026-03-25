namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о модели автомобиля.
/// </summary>
public class CarModelResponse
{
    /// <summary>
    /// Уникальный идентификатор модели.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название модели автомобиля.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор марки автомобиля.
    /// </summary>
    public Guid BrandId { get; set; }

    /// <summary>
    /// Название марки автомобиля.
    /// </summary>
    public string BrandName { get; set; } = null!;
}