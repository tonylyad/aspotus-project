namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Марка автомобиля.
/// Например: Toyota, BMW, Audi.
/// </summary>
public class CarBrand
{
    /// <summary>
    /// Уникальный идентификатор марки.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название марки автомобиля.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Список моделей, относящихся к данной марке.
    /// </summary>
    public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
}