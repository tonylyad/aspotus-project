namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Конкретный автомобиль в каталоге.
/// Используется как карточка автомобиля с характеристиками,
/// а также как сущность для подбора совместимых запчастей.
/// </summary>
public class Car
{
    /// <summary>
    /// Уникальный идентификатор автомобиля.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор марки автомобиля.
    /// </summary>
    public Guid BrandId { get; set; }

    /// <summary>
    /// Марка автомобиля.
    /// </summary>
    public CarBrand Brand { get; set; } = null!;

    /// <summary>
    /// Идентификатор модели автомобиля.
    /// </summary>
    public Guid ModelId { get; set; }

    /// <summary>
    /// Модель автомобиля.
    /// </summary>
    public CarModel Model { get; set; } = null!;

    /// <summary>
    /// Идентификатор поколения автомобиля.
    /// </summary>
    public Guid GenerationId { get; set; }

    /// <summary>
    /// Поколение автомобиля.
    /// </summary>
    public CarGeneration Generation { get; set; } = null!;

    /// <summary>
    /// Год выпуска конкретного автомобиля.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Тип кузова автомобиля.
    /// Например: Sedan, Suv, Hatchback.
    /// </summary>
    public string BodyType { get; set; } = null!;

    /// <summary>
    /// Объём двигателя в литрах.
    /// Например: 2.0, 2.5, 3.0.
    /// </summary>
    public decimal EngineVolume { get; set; }

    /// <summary>
    /// Тип топлива.
    /// Например: Petrol, Diesel, Hybrid, Electric.
    /// </summary>
    public string FuelType { get; set; } = null!;

    /// <summary>
    /// Тип коробки передач.
    /// Например: Manual, Automatic, Variator.
    /// </summary>
    public string TransmissionType { get; set; } = null!;

    /// <summary>
    /// Тип привода.
    /// Например: Fwd, Rwd, Awd.
    /// </summary>
    public string DriveType { get; set; } = null!;

    /// <summary>
    /// Список связей совместимости между автомобилем и запчастями.
    /// </summary>
    public ICollection<PartCompatibility> PartCompatibilities { get; set; } = new List<PartCompatibility>();
}