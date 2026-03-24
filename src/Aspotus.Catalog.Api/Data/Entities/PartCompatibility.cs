namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Связующая сущность между автомобилем и запчастью.
/// Показывает, что конкретная запчасть совместима с конкретным автомобилем.
/// </summary>
public class PartCompatibility
{
    /// <summary>
    /// Идентификатор запчасти.
    /// </summary>
    public Guid PartId { get; set; }

    /// <summary>
    /// Запчасть, участвующая в связи совместимости.
    /// </summary>
    public Part Part { get; set; } = null!;

    /// <summary>
    /// Идентификатор автомобиля.
    /// </summary>
    public Guid CarId { get; set; }

    /// <summary>
    /// Автомобиль, участвующий в связи совместимости.
    /// </summary>
    public Car Car { get; set; } = null!;
}