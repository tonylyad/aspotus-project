namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Поколение модели автомобиля.
/// Например: XV70, E210, G05.
/// </summary>
public class CarGeneration
{
    /// <summary>
    /// Уникальный идентификатор поколения.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название поколения модели.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Год начала выпуска поколения.
    /// </summary>
    public int YearFrom { get; set; }

    /// <summary>
    /// Год окончания выпуска поколения.
    /// Если поколение всё ещё выпускается, значение может быть null.
    /// </summary>
    public int? YearTo { get; set; }

    /// <summary>
    /// Идентификатор модели, к которой относится поколение.
    /// </summary>
    public Guid ModelId { get; set; }

    /// <summary>
    /// Модель автомобиля, к которой относится поколение.
    /// </summary>
    public CarModel Model { get; set; } = null!;

    /// <summary>
    /// Список конкретных автомобилей, относящихся к данному поколению.
    /// </summary>
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}
