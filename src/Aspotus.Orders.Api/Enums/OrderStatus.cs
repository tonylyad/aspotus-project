namespace Aspotus.Orders.Api.Enums;

/// <summary>
/// Статус заказа.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Заказ создан.
    /// </summary>
    Created = 1,

    /// <summary>
    /// Заказ в обработке.
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Заказ завершён.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Заказ отменён.
    /// </summary>
    Cancelled = 4
}