using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности запчасти в DTO ответа.
/// </summary>
public static class PartMapper
{
    /// <summary>
    /// Преобразует сущность запчасти в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность запчасти.</param>
    /// <returns>DTO ответа с информацией о запчасти.</returns>
    public static PartResponse ToResponse(Part entity)
    {
        return new PartResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Article = entity.Article,
            Description = entity.Description,
            Price = entity.Price,
            StockQuantity = entity.StockQuantity,
            IsOriginal = entity.IsOriginal,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category.Name,
            ManufacturerId = entity.ManufacturerId,
            ManufacturerName = entity.Manufacturer.Name
        };
    }
}