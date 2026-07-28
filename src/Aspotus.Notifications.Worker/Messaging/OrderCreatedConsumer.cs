using Aspotus.Notifications.Worker.Data;
using Aspotus.Shared.IntegrationEvents;
using Aspotus.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Aspotus.Notifications.Worker.Messaging;

/// <summary>
/// Получает уведомления о созданных заказах.
/// </summary>
public sealed class OrderCreatedConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<OrderCreatedConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            AutomaticRecoveryEnabled = true
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await DeclareTopologyAsync(channel, stoppingToken);
                await channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: 10,
                    global: false,
                    cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, args) =>
                {
                    var body = args.Body.ToArray();
                    await HandleMessageAsync(channel, args.DeliveryTag, body, stoppingToken);
                };

                await channel.BasicConsumeAsync(
                    RabbitMqTopology.NotificationsQueueName,
                    autoAck: false,
                    consumer,
                    stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Ошибка подключения consumer к RabbitMQ.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private static async Task DeclareTopologyAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            RabbitMqTopology.ExchangeName,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            RabbitMqTopology.NotificationsQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            RabbitMqTopology.NotificationsQueueName,
            RabbitMqTopology.ExchangeName,
            RabbitMqTopology.OrderCreatedRoutingKey,
            cancellationToken: cancellationToken);
    }

    private async Task HandleMessageAsync(
        IChannel channel,
        ulong deliveryTag,
        byte[] body,
        CancellationToken cancellationToken)
    {
        try
        {
            var integrationEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body)
                ?? throw new JsonException("Пустое событие OrderCreated.");

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

            if (!await context.ReceivedMessages.AnyAsync(
                    x => x.Id == integrationEvent.EventId,
                    cancellationToken))
            {
                logger.LogInformation(
                    "Уведомление: создан заказ {OrderId} типа {OrderType} для {CustomerName} ({CustomerEmail}) на сумму {TotalAmount}.",
                    integrationEvent.OrderId,
                    integrationEvent.OrderType,
                    integrationEvent.CustomerName,
                    integrationEvent.CustomerEmail,
                    integrationEvent.TotalAmount);

                context.ReceivedMessages.Add(new ReceivedMessage
                {
                    Id = integrationEvent.EventId,
                    ProcessedAtUtc = DateTime.UtcNow
                });
                await context.SaveChangesAsync(cancellationToken);
            }

            await channel.BasicAckAsync(
                deliveryTag,
                multiple: false,
                cancellationToken);
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Получено некорректное событие OrderCreated.");
            await channel.BasicRejectAsync(
                deliveryTag,
                requeue: false,
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await channel.BasicNackAsync(
                deliveryTag,
                multiple: false,
                requeue: true,
                CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Ошибка обработки события OrderCreated.");
            await channel.BasicNackAsync(
                deliveryTag,
                multiple: false,
                requeue: true,
                cancellationToken);
        }
    }
}
