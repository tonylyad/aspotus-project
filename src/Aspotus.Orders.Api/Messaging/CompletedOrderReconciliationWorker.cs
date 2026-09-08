using Aspotus.Orders.Api.Clients;
using Aspotus.Orders.Api.Data.Repositories.Interfaces;
using Aspotus.Orders.Api.Enums;

namespace Aspotus.Orders.Api.Messaging;

public sealed class CompletedOrderReconciliationWorker : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CompletedOrderReconciliationWorker> _logger;

    public CompletedOrderReconciliationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<CompletedOrderReconciliationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var catalogClient = scope.ServiceProvider.GetRequiredService<ICatalogInventoryClient>();
                var completedOrders = (await repository.GetAllAsync(stoppingToken))
                    .Where(order => order.Status == OrderStatus.Completed);

                foreach (var order in completedOrders)
                {
                    await catalogClient.CompleteAsync(order.Id, stoppingToken);
                }

                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Не удалось синхронизировать завершённые заказы с каталогом. Повтор через {Delay}.", RetryDelay);
                await Task.Delay(RetryDelay, stoppingToken);
            }
        }
    }
}
