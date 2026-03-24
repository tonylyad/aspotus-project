namespace Aspotus.Catalog.Api.Data.Entities;

/// <summary>
/// Категория запчастей.
/// Может быть корневой или вложенной.
/// Например: Engine, Brakes, Suspension.
/// </summary>
public class PartCategory
{
    /// <summary>
    /// Уникальный идентификатор категории.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название категории запчастей.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор родительской категории.
    /// Если null, значит категория корневая.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>
    /// Родительская категория.
    /// </summary>
    public PartCategory? ParentCategory { get; set; }

    /// <summary>
    /// Дочерние подкатегории.
    /// </summary>
    public ICollection<PartCategory> Children { get; set; } = new List<PartCategory>();

    /// <summary>
    /// Список запчастей, относящихся к данной категории.
    /// </summary>
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}