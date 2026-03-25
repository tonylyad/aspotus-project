namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о производителе запчастей.
/// </summary>
public class PartManufacturerResponse
{
    /// <summary>
    /// Уникальный идентификатор производителя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название производителя.
    /// </summary>
    public string Name { get; set; } = null!;
}