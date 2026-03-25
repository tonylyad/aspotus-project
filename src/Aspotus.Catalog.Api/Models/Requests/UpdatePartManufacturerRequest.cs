using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на обновление производителя запчастей.
/// </summary>
public class UpdatePartManufacturerRequest
{
    /// <summary>
    /// Новое название производителя запчастей.
    /// </summary>
    [Required(ErrorMessage = "Название производителя обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название производителя должно содержать от 2 до 100 символов.")]
    public string Name { get; set; } = null!;
}