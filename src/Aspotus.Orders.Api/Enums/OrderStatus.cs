namespace Aspotus.Orders.Api.Enums;

/// <summary>
/// Статус заказа.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Заказ создан.
    /// </summary>
    Created = 10,

    /// <summary>
    /// Заказ в обработке.
    /// </summary>
    Processing = 20,

    /// <summary>
    /// Заказ завершён.
    /// </summary>
    Completed = 30,

    /// <summary>
    /// Заказ отменён.
    /// </summary>
    Cancelled = 40,
}