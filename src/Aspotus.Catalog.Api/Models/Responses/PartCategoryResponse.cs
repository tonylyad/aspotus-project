namespace Aspotus.Catalog.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о категории запчастей.
/// </summary>
public class PartCategoryResponse
{
    /// <summary>
    /// Уникальный идентификатор категории.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название категории.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор родительской категории.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }
}