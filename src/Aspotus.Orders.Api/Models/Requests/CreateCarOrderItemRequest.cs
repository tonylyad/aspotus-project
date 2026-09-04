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
    public string? BrandName { get; set; }

    /// <summary>
    /// Название модели автомобиля на момент оформления заказа.
    /// </summary>
    public string? ModelName { get; set; }

    /// <summary>
    /// Название поколения автомобиля на момент оформления заказа.
    /// </summary>
    public string? GenerationName { get; set; }

    /// <summary>
    /// Год выпуска автомобиля.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Цена автомобиля на момент оформления заказа.
    /// </summary>
    public decimal Price { get; set; }
}
