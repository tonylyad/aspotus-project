using System.ComponentModel.DataAnnotations;

namespace Aspotus.Orders.Api.Models.Requests;

/// <summary>
/// Запрос на создание заказа автомобиля.
/// </summary>
public class CreateCarOrderRequest
{
    /// <summary>
    /// Идентификатор пользователя, оформляющего заказ.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    [EmailAddress(ErrorMessage = "Некорректный формат электронной почты.")]
    public string? UserEmail { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string? UserFullName { get; set; }

    /// <summary>
    /// Имя клиента, оформляющего заказ.
    /// </summary>
    [Required(ErrorMessage = "Имя клиента обязательно для заполнения.")]
    public string CustomerName { get; set; } = null!;

    /// <summary>
    /// Электронная почта клиента.
    /// </summary>
    [Required(ErrorMessage = "Электронная почта клиента обязательна для заполнения.")]
    [EmailAddress(ErrorMessage = "Некорректный формат электронной почты.")]
    public string CustomerEmail { get; set; } = null!;

    /// <summary>
    /// Телефон клиента.
    /// </summary>
    [Required(ErrorMessage = "Телефон клиента обязателен для заполнения.")]
    public string CustomerPhone { get; set; } = null!;

    /// <summary>
    /// Адрес доставки или оформления заказа.
    /// </summary>
    [Required(ErrorMessage = "Адрес обязателен для заполнения.")]
    public string DeliveryAddress { get; set; } = null!;

    /// <summary>
    /// Информация об автомобиле, который оформляется в заказ.
    /// </summary>
    [Required(ErrorMessage = "Информация об автомобиле обязательна.")]
    public CreateCarOrderItemRequest Car { get; set; } = null!;
}