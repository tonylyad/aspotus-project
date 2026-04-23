using System.ComponentModel.DataAnnotations;

namespace Aspotus.Orders.Api.Models.Requests;

/// <summary>
/// Запрос на создание заказа запчастей.
/// </summary>
public class CreatePartOrderRequest
{
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
    /// Список позиций заказа запчастей.
    /// Заказ должен содержать хотя бы одну позицию.
    /// </summary>
    [Required(ErrorMessage = "Список позиций заказа обязателен.")]
    [MinLength(1, ErrorMessage = "Заказ должен содержать хотя бы одну запчасть.")]
    public List<CreatePartOrderItemRequest> Items { get; set; } = new();
}