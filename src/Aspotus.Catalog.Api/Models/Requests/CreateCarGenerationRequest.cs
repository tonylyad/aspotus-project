using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на создание нового поколения автомобиля.
/// </summary>
public class CreateCarGenerationRequest : IValidatableObject
{
    /// <summary>
    /// Название поколения автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Название поколения обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Название поколения должно содержать от 1 до 100 символов.")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Год начала выпуска поколения.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Год начала выпуска должен быть в диапазоне от 1900 до 3000.")]
    public int YearFrom { get; set; }

    /// <summary>
    /// Год окончания выпуска поколения.
    /// Может быть не указан, если поколение всё ещё выпускается.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Год окончания выпуска должен быть в диапазоне от 1900 до 3000.")]
    public int? YearTo { get; set; }

    /// <summary>
    /// Идентификатор модели автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Модель автомобиля обязательна.")]
    public Guid ModelId { get; set; }

    /// <summary>
    /// Выполняет дополнительную валидацию модели запроса.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (YearTo.HasValue && YearTo.Value < YearFrom)
        {
            yield return new ValidationResult(
                "Год окончания выпуска не может быть меньше года начала выпуска.",
                new[] { nameof(YearTo) });
        }
    }
}