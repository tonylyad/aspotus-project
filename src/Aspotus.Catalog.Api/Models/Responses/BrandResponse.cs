namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о марке автомобиля.
/// </summary>
public class BrandResponse
{
    /// <summary>
    /// Уникальный идентификатор марки.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название марки автомобиля.
    /// </summary>
    public string Name { get; set; } = null!;
}