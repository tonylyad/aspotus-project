namespace Aspotus.Orders.Api.Models.Responses;

/// <summary>
/// Ответ с информацией о заказе.
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// Уникальный идентификатор заказа.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя из системы аутентификации.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Электронная почта пользователя на момент оформления заказа.
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Имя пользователя на момент оформления заказа.
    /// </summary>
    public string? UserFullName { get; set; }

    /// <summary>
    /// Имя клиента, оформившего заказ.
    /// </summary>
    public string CustomerName { get; set; } = null!;

    /// <summary>
    /// Электронная почта клиента.
    /// </summary>
    public string CustomerEmail { get; set; } = null!;

    /// <summary>
    /// Телефон клиента.
    /// </summary>
    public string CustomerPhone { get; set; } = null!;

    /// <summary>
    /// Адрес доставки или оформления заказа.
    /// </summary>
    public string DeliveryAddress { get; set; } = null!;

    /// <summary>
    /// Тип заказа в строковом представлении.
    /// </summary>
    public string OrderType { get; set; } = null!;

    /// <summary>
    /// Статус заказа в строковом представлении.
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Общая сумма заказа.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Дата и время создания заказа в UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Позиции заказа запчастей.
    /// </summary>
    public List<PartOrderItemResponse> PartItems { get; set; } = new();

    /// <summary>
    /// Позиции заказа автомобилей.
    /// </summary>
    public List<CarOrderItemResponse> CarItems { get; set; } = new();
}