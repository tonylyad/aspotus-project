using Aspotus.Orders.Api.Data.Context;
using Aspotus.Orders.Api.Options;
using Aspotus.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Aspotus.Orders.Api.Messaging;

/// <summary>
/// Публикует накопленные outbox-сообщения в RabbitMQ.
/// </summary>
public sealed class OutboxPublisher(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<OutboxPublisher> logger) : BackgroundService
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
                await using var channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(
                        publisherConfirmationsEnabled: true,
                        publisherConfirmationTrackingEnabled: true),
                    stoppingToken);

                await channel.ExchangeDeclareAsync(
                    RabbitMqTopology.ExchangeName,
                    ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: stoppingToken);
                await channel.QueueDeclareAsync(
                    RabbitMqTopology.NotificationsQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken);
                await channel.QueueBindAsync(
                    RabbitMqTopology.NotificationsQueueName,
                    RabbitMqTopology.ExchangeName,
                    RabbitMqTopology.OrderCreatedRoutingKey,
                    cancellationToken: stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var publishedAny = await PublishBatchAsync(channel, stoppingToken);

                    if (!publishedAny)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Ошибка публикации outbox в RabbitMQ.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task<bool> PublishBatchAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var messages = await context.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                    MessageId = message.Id.ToString(),
                    Type = message.Type
                };

                await channel.BasicPublishAsync(
                    exchange: RabbitMqTopology.ExchangeName,
                    routingKey: message.Type,
                    mandatory: true,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(message.Payload),
                    cancellationToken: cancellationToken);

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception exception)
            {
                message.Attempts++;
                message.Error = exception.Message;
                await context.SaveChangesAsync(cancellationToken);
                throw;
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        return messages.Count > 0;
    }
}
