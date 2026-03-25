using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на обновление марки автомобиля.
/// </summary>
public class UpdateBrandRequest
{
    /// <summary>
    /// Новое название марки автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Название марки обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название марки должно содержать от 2 до 100 символов.")]
    public string Name { get; set; } = null!;
}