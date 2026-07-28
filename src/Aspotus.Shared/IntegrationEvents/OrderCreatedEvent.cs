namespace Aspotus.Shared.IntegrationEvents;

/// <summary>
/// Интеграционное событие о создании заказа.
/// </summary>
public sealed record OrderCreatedEvent(
    Guid EventId,
    Guid OrderId,
    Guid? UserId,
    string? UserEmail,
    string CustomerEmail,
    string CustomerName,
    string OrderType,
    decimal TotalAmount,
    DateTime CreatedAtUtc);
