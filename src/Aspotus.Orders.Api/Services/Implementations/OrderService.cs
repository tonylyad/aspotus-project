using System.Text.Json;
using Aspotus.Orders.Api.Clients;
using Aspotus.Orders.Api.Data.Entities;
using Aspotus.Orders.Api.Data.Repositories.Interfaces;
using Aspotus.Orders.Api.Enums;
using Aspotus.Orders.Api.Exceptions;
using Aspotus.Orders.Api.Mappers;
using Aspotus.Orders.Api.Models.Requests;
using Aspotus.Orders.Api.Models.Responses;
using Aspotus.Orders.Api.Services.Interfaces;
using Aspotus.Shared.IntegrationEvents;
using Aspotus.Shared.Messaging;

namespace Aspotus.Orders.Api.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICatalogInventoryClient _catalogInventoryClient;

    public OrderService(IOrderRepository orderRepository, ICatalogInventoryClient catalogInventoryClient)
    {
        _orderRepository = orderRepository;
        _catalogInventoryClient = catalogInventoryClient;
    }

    public async Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await _orderRepository.GetAllAsync(cancellationToken)).Select(OrderMapper.ToResponse).ToList();

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : OrderMapper.ToResponse(order);
    }

    public async Task<IReadOnlyCollection<OrderResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        (await _orderRepository.GetByUserIdAsync(userId, cancellationToken)).Select(OrderMapper.ToResponse).ToList();

    public async Task<OrderResponse> CreatePartOrderAsync(
        CreatePartOrderRequest request,
        string? userId,
        string? userEmail,
        string? userFullName,
        CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new ValidationException("Заказ должен содержать хотя бы одну запчасть.");

        var orderId = Guid.NewGuid();
        var parsedUserId = ParseUserId(userId);
        var reservation = await _catalogInventoryClient.ReserveAsync(
            orderId,
            parsedUserId,
            request.Items.Select(x => new CatalogReservationItemRequest("Part", x.PartId, x.Quantity)).ToList(),
            cancellationToken);

        var order = CreateOrderBase(orderId, parsedUserId, request.CustomerName, request.CustomerEmail,
            request.CustomerPhone, request.DeliveryAddress, userEmail, userFullName, OrderType.Part);

        order.PartItems = reservation.Items.Select(x => new PartOrderItem
        {
            Id = Guid.NewGuid(),
            PartId = x.ProductId,
            PartName = x.Name,
            PartArticle = x.Article ?? string.Empty,
            UnitPrice = x.UnitPrice,
            Quantity = x.Quantity,
            TotalPrice = x.UnitPrice * x.Quantity
        }).ToList();
        order.TotalAmount = order.PartItems.Sum(x => x.TotalPrice);

        await PersistWithCompensationAsync(order, cancellationToken);
        return OrderMapper.ToResponse(order);
    }

    public async Task<OrderResponse> CreateCarOrderAsync(
        CreateCarOrderRequest request,
        string? userId,
        string? userEmail,
        string? userFullName,
        CancellationToken cancellationToken = default)
    {
        if (request.Car is null) throw new ValidationException("Информация об автомобиле обязательна.");

        var orderId = Guid.NewGuid();
        var parsedUserId = ParseUserId(userId);
        var reservation = await _catalogInventoryClient.ReserveAsync(
            orderId,
            parsedUserId,
            new[] { new CatalogReservationItemRequest("Car", request.Car.CarId, 1) },
            cancellationToken);
        var reservedCar = reservation.Items.Single();

        var order = CreateOrderBase(orderId, parsedUserId, request.CustomerName, request.CustomerEmail,
            request.CustomerPhone, request.DeliveryAddress, userEmail, userFullName, OrderType.Car);
        order.CarItems = new List<CarOrderItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CarId = reservedCar.ProductId,
                BrandName = reservedCar.BrandName ?? reservedCar.Name,
                ModelName = reservedCar.ModelName ?? string.Empty,
                GenerationName = reservedCar.GenerationName ?? string.Empty,
                Year = reservedCar.Year ?? 0,
                Price = reservedCar.UnitPrice
            }
        };
        order.TotalAmount = reservedCar.UnitPrice;

        await PersistWithCompensationAsync(order, cancellationToken);
        return OrderMapper.ToResponse(order);
    }

    public async Task<OrderResponse?> UpdateStatusAsync(
        Guid id,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null) return null;

        if (!Enum.IsDefined(request.Status))
            throw new ValidationException("Указан неизвестный статус заказа.");

        if (order.Status == request.Status)
        {
            if (request.Status == OrderStatus.Cancelled)
                await _catalogInventoryClient.ReleaseAsync(id, cancellationToken);

            return OrderMapper.ToResponse(order);
        }

        var transitionAllowed = order.Status switch
        {
            OrderStatus.Created => request.Status is OrderStatus.Processing or OrderStatus.Cancelled,
            OrderStatus.Processing => request.Status is OrderStatus.Completed or OrderStatus.Cancelled,
            _ => false
        };

        if (!transitionAllowed)
            throw new ValidationException($"Нельзя изменить статус заказа с {order.Status} на {request.Status}.");

        order.Status = request.Status;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        if (request.Status == OrderStatus.Cancelled)
            await _catalogInventoryClient.ReleaseAsync(id, cancellationToken);

        return OrderMapper.ToResponse(order);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null) return false;
        if (order.Status == OrderStatus.Completed)
            throw new ValidationException("Завершённый заказ нельзя удалить: товар уже продан.");

        await _orderRepository.DeleteAsync(id, cancellationToken);
        await _catalogInventoryClient.ReleaseAsync(id, cancellationToken);
        return true;
    }

    private async Task PersistWithCompensationAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            await _orderRepository.AddAsync(order, CreateOutboxMessage(order), cancellationToken);
        }
        catch
        {
            await _catalogInventoryClient.ReleaseAsync(order.Id, CancellationToken.None);
            throw;
        }
    }

    private static Order CreateOrderBase(
        Guid id, Guid? userId, string customerName, string customerEmail, string customerPhone,
        string deliveryAddress, string? userEmail, string? userFullName, OrderType orderType) => new()
    {
        Id = id,
        UserId = userId,
        UserEmail = string.IsNullOrWhiteSpace(userEmail) ? null : userEmail.Trim(),
        UserFullName = string.IsNullOrWhiteSpace(userFullName) ? null : userFullName.Trim(),
        CustomerName = customerName.Trim(),
        CustomerEmail = customerEmail.Trim(),
        CustomerPhone = customerPhone.Trim(),
        DeliveryAddress = deliveryAddress.Trim(),
        OrderType = orderType,
        Status = OrderStatus.Created,
        CreatedAtUtc = DateTime.UtcNow
    };

    private static Guid? ParseUserId(string? userId) =>
        Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : null;

    private static OutboxMessage CreateOutboxMessage(Order order)
    {
        var integrationEvent = new OrderCreatedEvent(
            Guid.NewGuid(), order.Id, order.UserId, order.UserEmail, order.CustomerEmail,
            order.CustomerName, order.OrderType.ToString(), order.TotalAmount, order.CreatedAtUtc);

        return new OutboxMessage
        {
            Id = integrationEvent.EventId,
            Type = RabbitMqTopology.OrderCreatedRoutingKey,
            Payload = JsonSerializer.Serialize(integrationEvent),
            OccurredAtUtc = integrationEvent.CreatedAtUtc
        };
    }
}
