namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Номер заменителя запчасти.
/// Используется для хранения альтернативных артикулов,
/// по которым пользователь может искать совместимые детали.
/// </summary>
public class PartReplacement
{
    /// <summary>
    /// Уникальный идентификатор заменителя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор запчасти, к которой относится заменитель.
    /// </summary>
    public Guid PartId { get; set; }

    /// <summary>
    /// Запчасть, к которой относится заменитель.
    /// </summary>
    public Part Part { get; set; } = null!;

    /// <summary>
    /// Артикул заменителя.
    /// </summary>
    public string ReplacementArticle { get; set; } = null!;
}