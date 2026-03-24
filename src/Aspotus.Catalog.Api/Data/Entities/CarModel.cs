namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Модель автомобиля внутри конкретной марки.
/// Например: Camry, X5, A6.
/// </summary>
public class CarModel
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
    /// Идентификатор марки, к которой относится модель.
    /// </summary>
    public Guid BrandId { get; set; }

    /// <summary>
    /// Марка автомобиля, к которой относится модель.
    /// </summary>
    public CarBrand Brand { get; set; } = null!;

    /// <summary>
    /// Список поколений данной модели.
    /// </summary>
    public ICollection<CarGeneration> Generations { get; set; } = new List<CarGeneration>();
}