using Aspotus.Orders.Api.Enums;

namespace Aspotus.Orders.Api.Data.Entities;

/// <summary>
/// Заказ клиента.
/// Может содержать либо позиции запчастей, либо позиции автомобилей.
/// </summary>
public class Order
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
    /// Имя клиента.
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
    /// Тип заказа.
    /// </summary>
    public OrderType OrderType { get; set; }

    /// <summary>
    /// Статус заказа.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Общая сумма заказа.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Дата и время создания заказа в UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Позиции заказа на запчасти.
    /// </summary>
    public ICollection<PartOrderItem> PartItems { get; set; } = new List<PartOrderItem>();

    /// <summary>
    /// Позиции заказа на автомобили.
    /// </summary>
    public ICollection<CarOrderItem> CarItems { get; set; } = new List<CarOrderItem>();
}