using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности марки автомобиля в DTO ответа.
/// </summary>
public static class BrandMapper
{
    /// <summary>
    /// Преобразует сущность марки автомобиля в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность марки автомобиля.</param>
    /// <returns>DTO ответа с информацией о марке.</returns>
    public static BrandResponse ToResponse(CarBrand entity)
    {
        return new BrandResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}