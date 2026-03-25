using System.ComponentModel.DataAnnotations;

namespace Aspotus.Orders.Api.Models.Requests;

/// <summary>
/// Позиция запроса на создание заказа автомобиля.
/// </summary>
public class CreateCarOrderItemRequest
{
    /// <summary>
    /// Идентификатор автомобиля из каталога.
    /// </summary>
    [Required(ErrorMessage = "Идентификатор автомобиля обязателен.")]
    public Guid CarId { get; set; }

    /// <summary>
    /// Название марки автомобиля на момент оформления заказа.
    /// </summary>
    [Required(ErrorMessage = "Название марки обязательно для заполнения.")]
    public string BrandName { get; set; } = null!;

    /// <summary>
    /// Название модели автомобиля на момент оформления заказа.
    /// </summary>
    [Required(ErrorMessage = "Название модели обязательно для заполнения.")]
    public string ModelName { get; set; } = null!;

    /// <summary>
    /// Название поколения автомобиля на момент оформления заказа.
    /// </summary>
    [Required(ErrorMessage = "Название поколения обязательно для заполнения.")]
    public string GenerationName { get; set; } = null!;

    /// <summary>
    /// Год выпуска автомобиля.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Год выпуска должен быть в диапазоне от 1900 до 3000.")]
    public int Year { get; set; }

    /// <summary>
    /// Цена автомобиля на момент оформления заказа.
    /// </summary>
    [Range(0.01, 1000000000, ErrorMessage = "Цена автомобиля должна быть больше 0.")]
    public decimal Price { get; set; }
}