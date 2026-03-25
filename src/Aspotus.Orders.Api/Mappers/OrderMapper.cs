using Aspotus.Orders.Api.Data.Entities;
using Aspotus.Orders.Api.Models.Responses;

namespace Aspotus.Orders.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущностей заказов в DTO ответов.
/// </summary>
public static class OrderMapper
{
    /// <summary>
    /// Преобразует сущность заказа в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность заказа.</param>
    /// <returns>DTO ответа с информацией о заказе.</returns>
    public static OrderResponse ToResponse(Order entity)
    {
        return new OrderResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            UserEmail = entity.UserEmail,
            UserFullName = entity.UserFullName,
            CustomerName = entity.CustomerName,
            CustomerEmail = entity.CustomerEmail,
            CustomerPhone = entity.CustomerPhone,
            DeliveryAddress = entity.DeliveryAddress,
            OrderType = entity.OrderType.ToString(),
            Status = entity.Status.ToString(),
            TotalAmount = entity.TotalAmount,
            CreatedAtUtc = entity.CreatedAtUtc,
            PartItems = entity.PartItems.Select(ToPartOrderItemResponse).ToList(),
            CarItems = entity.CarItems.Select(ToCarOrderItemResponse).ToList()
        };
    }

    /// <summary>
    /// Преобразует сущность позиции заказа запчастей в DTO ответа.
    /// </summary>
    public static PartOrderItemResponse ToPartOrderItemResponse(PartOrderItem entity)
    {
        return new PartOrderItemResponse
        {
            Id = entity.Id,
            PartId = entity.PartId,
            PartName = entity.PartName,
            PartArticle = entity.PartArticle,
            UnitPrice = entity.UnitPrice,
            Quantity = entity.Quantity,
            TotalPrice = entity.TotalPrice
        };
    }

    /// <summary>
    /// Преобразует сущность позиции заказа автомобиля в DTO ответа.
    /// </summary>
    public static CarOrderItemResponse ToCarOrderItemResponse(CarOrderItem entity)
    {
        return new CarOrderItemResponse
        {
            Id = entity.Id,
            CarId = entity.CarId,
            BrandName = entity.BrandName,
            ModelName = entity.ModelName,
            GenerationName = entity.GenerationName,
            Year = entity.Year,
            Price = entity.Price
        };
    }
}