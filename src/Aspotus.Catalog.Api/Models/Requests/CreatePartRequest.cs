using System.ComponentModel.DataAnnotations;
using Aspotus.Catalog.Api.Enums;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на создание новой запчасти.
/// </summary>
public class CreatePartRequest : IValidatableObject
{
    /// <summary>
    /// Название запчасти.
    /// </summary>
    [Required(ErrorMessage = "Название запчасти обязательно для заполнения.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Название запчасти должно содержать от 2 до 200 символов.")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Артикул запчасти.
    /// </summary>
    [Required(ErrorMessage = "Артикул обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Артикул должен содержать от 2 до 100 символов.")]
    public string Article { get; set; } = null!;

    /// <summary>
    /// Описание запчасти.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Описание запчасти не должно превышать 1000 символов.")]
    public string? Description { get; set; }

    /// <summary>
    /// Цена запчасти.
    /// </summary>
    [Range(0.01, 1000000000, ErrorMessage = "Цена запчасти должна быть больше 0.")]
    public decimal Price { get; set; }

    /// <summary>
    /// Количество на складе.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Количество на складе не может быть отрицательным.")]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Признак оригинальной запчасти.
    /// </summary>
    public bool IsOriginal { get; set; }

    /// <summary>
    /// Тип состояния запчасти.
    /// </summary>
    [Required(ErrorMessage = "Тип состояния запчасти обязателен.")]
    public PartConditionType ConditionType { get; set; }

    /// <summary>
    /// Процент состояния БУ-запчасти.
    /// </summary>
    [Range(0, 100, ErrorMessage = "Процент состояния должен быть в диапазоне от 0 до 100.")]
    public int? ConditionPercent { get; set; }

    /// <summary>
    /// Описание состояния БУ-запчасти.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Описание состояния не должно превышать 1000 символов.")]
    public string? ConditionDescription { get; set; }

    /// <summary>
    /// Пробег автомобиля на момент снятия БУ-запчасти.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Пробег не может быть отрицательным.")]
    public int? MileageAtRemoval { get; set; }

    /// <summary>
    /// Список артикулов заменителей.
    /// </summary>
    public List<string> ReplacementArticles { get; set; } = new();

    /// <summary>
    /// Идентификатор категории запчасти.
    /// </summary>
    [Required(ErrorMessage = "Категория запчасти обязательна.")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Идентификатор производителя запчасти.
    /// </summary>
    [Required(ErrorMessage = "Производитель запчасти обязателен.")]
    public Guid ManufacturerId { get; set; }

    public List<CatalogImageRequest> Images { get; set; } = new();

    /// <summary>
    /// Выполняет дополнительную бизнес-валидацию запроса.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ConditionType == PartConditionType.New)
        {
            if (ConditionPercent.HasValue || !string.IsNullOrWhiteSpace(ConditionDescription) || MileageAtRemoval.HasValue)
            {
                yield return new ValidationResult(
                    "Для новой запчасти нельзя указывать состояние, описание состояния и пробег снятия.",
                    new[] { nameof(ConditionPercent), nameof(ConditionDescription), nameof(MileageAtRemoval) });
            }
        }

        if (ConditionType == PartConditionType.Used && !ConditionPercent.HasValue)
        {
            yield return new ValidationResult(
                "Для БУ-запчасти необходимо указать процент состояния.",
                new[] { nameof(ConditionPercent) });
        }
    }
}
