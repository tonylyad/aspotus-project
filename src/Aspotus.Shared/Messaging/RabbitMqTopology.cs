namespace Aspotus.Shared.Messaging;

/// <summary>
/// Имена элементов топологии RabbitMQ.
/// </summary>
public static class RabbitMqTopology
{
    public const string ExchangeName = "aspotus.events";
    public const string OrderCreatedRoutingKey = "orders.created.v1";
    public const string NotificationsQueueName = "notifications.order-created";
}
