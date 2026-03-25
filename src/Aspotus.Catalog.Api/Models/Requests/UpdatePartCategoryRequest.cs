using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на обновление категории запчастей.
/// </summary>
public class UpdatePartCategoryRequest
{
    /// <summary>
    /// Новое название категории запчастей.
    /// </summary>
    [Required(ErrorMessage = "Название категории обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название категории должно содержать от 2 до 100 символов.")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор родительской категории.
    /// Если не указан, категория становится корневой.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }
}