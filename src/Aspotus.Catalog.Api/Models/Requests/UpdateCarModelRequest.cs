using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на обновление модели автомобиля.
/// </summary>
public class UpdateCarModelRequest
{
    /// <summary>
    /// Новое название модели автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Название модели обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название модели должно содержать от 2 до 100 символов.")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор марки автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Марка автомобиля обязательна.")]
    public Guid BrandId { get; set; }
}