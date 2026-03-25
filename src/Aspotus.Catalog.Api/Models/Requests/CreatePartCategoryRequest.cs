using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на создание новой категории запчастей.
/// </summary>
public class CreatePartCategoryRequest
{
    /// <summary>
    /// Название новой категории запчастей.
    /// </summary>
    [Required(ErrorMessage = "Название категории обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название категории должно содержать от 2 до 100 символов.")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор родительской категории.
    /// Если не указан, создаётся корневая категория.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }
}