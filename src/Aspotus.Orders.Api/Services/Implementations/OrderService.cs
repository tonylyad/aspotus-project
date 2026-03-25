using Aspotus.Orders.Api.Data.Entities;
using Aspotus.Orders.Api.Data.Repositories.Interfaces;
using Aspotus.Orders.Api.Enums;
using Aspotus.Orders.Api.Exceptions;
using Aspotus.Orders.Api.Mappers;
using Aspotus.Orders.Api.Models.Requests;
using Aspotus.Orders.Api.Models.Responses;
using Aspotus.Orders.Api.Services.Interfaces;

namespace Aspotus.Orders.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с заказами.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса заказов.
    /// </summary>
    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders
            .Select(OrderMapper.ToResponse)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        return OrderMapper.ToResponse(order);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrderResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);

        return orders
            .Select(OrderMapper.ToResponse)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<OrderResponse> CreatePartOrderAsync(
        CreatePartOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ValidationException("Заказ должен содержать хотя бы одну запчасть.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            UserEmail = string.IsNullOrWhiteSpace(request.UserEmail) ? null : request.UserEmail.Trim(),
            UserFullName = string.IsNullOrWhiteSpace(request.UserFullName) ? null : request.UserFullName.Trim(),
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            DeliveryAddress = request.DeliveryAddress.Trim(),
            OrderType = OrderType.Part,
            Status = OrderStatus.Created,
            CreatedAtUtc = DateTime.UtcNow,
            PartItems = request.Items.Select(x => new PartOrderItem
            {
                Id = Guid.NewGuid(),
                PartId = x.PartId,
                PartName = x.PartName.Trim(),
                PartArticle = x.PartArticle.Trim(),
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                TotalPrice = x.UnitPrice * x.Quantity
            }).ToList()
        };

        order.TotalAmount = order.PartItems.Sum(x => x.TotalPrice);

        await _orderRepository.AddAsync(order, cancellationToken);

        return OrderMapper.ToResponse(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponse> CreateCarOrderAsync(
        CreateCarOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Car is null)
        {
            throw new ValidationException("Информация об автомобиле обязательна.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            UserEmail = string.IsNullOrWhiteSpace(request.UserEmail) ? null : request.UserEmail.Trim(),
            UserFullName = string.IsNullOrWhiteSpace(request.UserFullName) ? null : request.UserFullName.Trim(),
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            DeliveryAddress = request.DeliveryAddress.Trim(),
            OrderType = OrderType.Car,
            Status = OrderStatus.Created,
            CreatedAtUtc = DateTime.UtcNow,
            CarItems = new List<CarOrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CarId = request.Car.CarId,
                    BrandName = request.Car.BrandName.Trim(),
                    ModelName = request.Car.ModelName.Trim(),
                    GenerationName = request.Car.GenerationName.Trim(),
                    Year = request.Car.Year,
                    Price = request.Car.Price
                }
            }
        };

        order.TotalAmount = order.CarItems.Sum(x => x.Price);

        await _orderRepository.AddAsync(order, cancellationToken);

        return OrderMapper.ToResponse(order);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (existingOrder is null)
        {
            return false;
        }

        await _orderRepository.DeleteAsync(id, cancellationToken);

        return true;
    }
}