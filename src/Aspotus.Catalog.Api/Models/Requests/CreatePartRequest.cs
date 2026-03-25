using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на создание новой запчасти.
/// </summary>
public class CreatePartRequest
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
    /// Идентификатор категории запчасти.
    /// </summary>
    [Required(ErrorMessage = "Категория запчасти обязательна.")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Идентификатор производителя запчасти.
    /// </summary>
    [Required(ErrorMessage = "Производитель запчасти обязателен.")]
    public Guid ManufacturerId { get; set; }
}