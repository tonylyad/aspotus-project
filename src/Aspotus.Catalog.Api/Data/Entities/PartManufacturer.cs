namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Производитель запчастей.
/// Например: Bosch, Brembo, KYB.
/// </summary>
public class PartManufacturer
{
    /// <summary>
    /// Уникальный идентификатор производителя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название производителя запчастей.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Список запчастей данного производителя.
    /// </summary>
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}
